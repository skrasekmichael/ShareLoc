﻿using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ShareLoc.Client.App.Messages;
using ShareLoc.Client.App.Models;
using ShareLoc.Client.App.Services;
using ShareLoc.Client.BL.Services;
using ShareLoc.Shared.Common.Models;

namespace ShareLoc.Client.App.ViewModels;

public sealed partial class PlaceDetailPageViewModel : BaseViewModel
{
	private readonly ModelMapper _modelMapper;
	private readonly IAlertService _alertService;
	private readonly ApiClient _apiClient;
	private readonly LocalDbService _localDbService;
	private readonly INavigationService _navigationService;
	private readonly IMediator _mediator;
	private readonly PlaceSharingService _sharePlaceService;

	[ObservableProperty]
	private List<GuessModel> _guesses = [];

	public PlaceDetailViewModel PlaceDetailViewModel { get; }
	public PlaceModel? PlaceModel
	{
		get => PlaceDetailViewModel.Model;
		set
		{
			PlaceDetailViewModel.Model = value;
			OnPropertyChanged(nameof(PlaceModel));
		}
	}

	[ObservableProperty]
	private bool _isSharing = false;

	public PlaceDetailPageViewModel(PlaceDetailViewModel placeDetailViewModel, ModelMapper modelMapper, IAlertService alertService, ApiClient apiClient, LocalDbService localDbService, INavigationService navigationService, IMediator mediator, PlaceSharingService sharePlaceService)
	{
		PlaceDetailViewModel = placeDetailViewModel;
		_modelMapper = modelMapper;
		_alertService = alertService;
		_apiClient = apiClient;
		_localDbService = localDbService;
		_navigationService = navigationService;
		_mediator = mediator;
		_sharePlaceService = sharePlaceService;
	}

	protected override async Task LoadAsync(CancellationToken ct)
	{
		await Refresh(ct);
	}

	[RelayCommand]
	private async Task Refresh(CancellationToken ct)
	{
		if (PlaceModel is null)
			return;

		IsLoading = true;

		List<GuessResponse> guesses = [];
		if (PlaceModel.IsShared)
		{
			var response = await _apiClient.GetGuessesAsync(PlaceModel.ServerId, ct);
			await response.Match<ValueTask>(
				data =>
				{
					guesses = data;
					return ValueTask.CompletedTask;
				},
				async notFound =>
				{
					_localDbService.RemoveServerId(PlaceModel.LocalId);
					_mediator.Publish(new PlaceSharingStateChangedMessage(PlaceModel.LocalId, Guid.Empty));
					await _alertService.ShowAlertAsync("Place not found.", "Place was removed from the server.", "OK");
				},
				async unexpectedError => await _alertService.ShowAlertAsync("Unexpected error.", "There was an unexpected error.", "OK")
			);
		}

		await Dispatch(() => Guesses = guesses.Select(guess => _modelMapper.Map(guess)).ToList());
		await Task.Delay(800, ct); //artificial delay to simulate loading process
		IsLoading = false;
	}

	[RelayCommand]
	private Task ShowGuess(GuessModel guess)
	{
		if (guess.IsSelected)
		{
			guess.IsSelected = false;
			return PlaceDetailViewModel.HideGuessAsync();
		}
		else
		{
			Guesses.ForEach(guess => guess.IsSelected = false);
			guess.IsSelected = true;
			return PlaceDetailViewModel.ShowGuessAsync(guess);
		}
	}

	[RelayCommand]
	private async Task Delete(CancellationToken ct)
	{
		if (PlaceModel is null)
			return;

		var confirm = await _alertService.ShowConfirmationAsync("Delete", "Do you want to delete this place?", "YES", "NO");
		if (!confirm)
			return;

		_localDbService.Delete(PlaceModel.LocalId);

		_mediator.Publish(new PlaceDeletedMessage(PlaceModel.LocalId));
		await _navigationService.GoBackAsync();
	}

	[RelayCommand]
	private async Task UpdateMessage(CancellationToken ct)
	{
		if (PlaceModel is null)
			return;

		var result = _localDbService.UpdateMessage(PlaceModel.LocalId, PlaceModel.Message);
		await result.Match<ValueTask>(
			success =>
			{
				PlaceModel.IsModified = false;
				_mediator.Publish(new PlaceMessageUpdatedMessage(PlaceModel.LocalId, PlaceModel.Message));
				Toast.Make("Message successfully updated.", ToastDuration.Short).Show();
				return ValueTask.CompletedTask;
			},
			async notFound => await _alertService.ShowAlertAsync("Place not found.", "Bug! this this place should not have been available after deleting.", "OK"),
			async unexpectedError => await _alertService.ShowAlertAsync("Unexpected error.", "There was an unexpected error.", "OK")
		);
	}

	[RelayCommand]
	private async Task ShareUrl(CancellationToken ct)
	{
		if (PlaceModel is null)
			return;

		IsSharing = true;

		if (PlaceModel.IsShared)
		{
			//place is stored on the server, just share url
			await _sharePlaceService.SharePlaceUrlAsync(PlaceModel);
			IsSharing = false;
			return;
		}

		var response = await _apiClient.CreatePlaceAsync(_modelMapper.Map(PlaceModel), ct);
		await response.Match<ValueTask>(
			async (serverId) =>
			{
				PlaceModel.ServerId = serverId;
				_localDbService.SharePlace(PlaceModel.LocalId, serverId, DateTime.UtcNow);
				_mediator.Publish(new PlaceSharingStateChangedMessage(PlaceModel.LocalId, serverId));
				await _sharePlaceService.SharePlaceUrlAsync(PlaceModel);
			},
			async validationErrors => await _alertService.ShowAlertAsync("Error", $"Invalid values given: {validationErrors}", "OK"),
			async error => await _alertService.ShowAlertAsync("Error", $"Error while creating place: {error}", "OK")
		);

		IsSharing = false;
	}
}
