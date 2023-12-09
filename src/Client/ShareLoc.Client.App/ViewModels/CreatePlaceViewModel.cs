﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.Options;

using ShareLoc.Client.App.Messages;
using ShareLoc.Client.App.Models;
using ShareLoc.Client.App.Services;
using ShareLoc.Client.BL;
using ShareLoc.Client.BL.Services;

namespace ShareLoc.Client.App.ViewModels;

public sealed partial class CreatePlaceViewModel : BaseViewModel
{
	private readonly INavigationService _navigationService;
	private readonly IAlertService _alertService;
	private readonly ApiClient _apiClient;
	private readonly LocalDbService _localDbService;
	private readonly IOptions<ServerOptions> _serverOptions;
	private readonly ModelMapper _modelMapper;
	private readonly IMediator _mediator;

	[ObservableProperty]
	private PlaceDetailViewModel _placeDetailViewModel;

	public CreatePlaceViewModel(ApiClient apiClient, INavigationService navigationService, LocalDbService localDbService, IAlertService alertService, PlaceDetailViewModel placeDetailViewModel, IOptions<ServerOptions> serverOptions, ModelMapper modelMapper, IMediator mediator)
	{
		_apiClient = apiClient;
		_navigationService = navigationService;
		_localDbService = localDbService;
		_alertService = alertService;
		_serverOptions = serverOptions;
		_modelMapper = modelMapper;

		PlaceDetailViewModel = placeDetailViewModel;
		_mediator = mediator;
	}

	protected override async Task LoadAsync(CancellationToken ct)
	{
		var imageBuffer = await Dispatch(() => TakePhotoAsync(ct));
		if (imageBuffer is null)
		{
			await Dispatch(() => _navigationService.GoToHomeAsync());
			return;
		}

		var location = await Dispatch(() => GetLocationAsync(ct));
		if (location is null)
		{
			await Dispatch(() => _navigationService.GoToHomeAsync());
			return;
		}

		PlaceDetailViewModel.Model = new PlaceModel()
		{
			Image = imageBuffer,
			Latitude = location.Latitude,
			Longitude = location.Longitude
		};
	}

	private async Task<byte[]?> TakePhotoAsync(CancellationToken ct)
	{
		if (!MediaPicker.IsCaptureSupported)
		{
			await _alertService.ShowAlertAsync("Error", "Camera not supported", "OK");
			return null;
		}

		var photo = await MediaPicker.CapturePhotoAsync();
		if (photo is null)
			return null;

		using var stream = await photo.OpenReadAsync();
		var buffer = new byte[stream.Length];
		await stream.ReadAsync(buffer.AsMemory(0, (int)stream.Length), ct);
		return buffer;
	}

	private async Task<Location?> GetLocationAsync(CancellationToken ct)
	{
		Location? location = null;
		var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(5));

		try
		{
			location = await Geolocation.GetLocationAsync(request, ct);
			if (location is null)
			{
				await _alertService.ShowAlertAsync("Error", "Unable to obtain the current location.", "OK");
			}
		}
		catch (UnauthorizedAccessException)
		{
			await _alertService.ShowAlertAsync("Error", "Geolocation access denied.", "OK");
		}
		catch (Exception ex)
		{
			await _alertService.ShowAlertAsync("Error", ex.Message, "OK");
		}

		return location;
	}

	[RelayCommand]
	private Task Cancel() => _navigationService.GoToHomeAsync();

	[RelayCommand]
	private async Task Save(CancellationToken ct)
	{
		if (PlaceDetailViewModel.Model is null)
			return;

		var entityResult = await _localDbService.SavePlaceAsync(_modelMapper.Map(PlaceDetailViewModel.Model), ct);

		entityResult.Switch(
			async entity =>
			{
				PlaceDetailViewModel.Model.LocalId = entity.Value.LocalId;
				_mediator.Publish(new PlaceStoredMessage(PlaceDetailViewModel.Model));
				await _navigationService.GoToHomeAsync();
			},
			async error => await _alertService.ShowAlertAsync("Error", $"Failed to save Place", "OK")
		);
	}

	[RelayCommand]
	private async Task ShareUrl(CancellationToken ct)
	{
		if (PlaceDetailViewModel.Model is null)
			return;

		var response = await _apiClient.CreatePlaceAsync(_modelMapper.Map(PlaceDetailViewModel.Model), ct);

		response.Switch(
			async (serverId) =>
			{
				await _localDbService.SharePlaceAsync(PlaceDetailViewModel.Model.LocalId, serverId, DateTime.UtcNow);
				await Share.RequestAsync(new ShareTextRequest
				{
					Text = $"{_serverOptions.Value.Address}/{serverId}",
					Title = PlaceDetailViewModel.Model.Message
				});
				await _navigationService.GoToHomeAsync();
			},
			async validationErrors => await _alertService.ShowAlertAsync("Error", $"Invalid values given: {validationErrors}", "OK"),
			async error => await _alertService.ShowAlertAsync("Error", $"Error while creating place: {error}", "OK")
		);
	}
}
