using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.Options;

using ShareLoc.Client.App.Messages;
using ShareLoc.Client.App.Models;
using ShareLoc.Client.App.Services;
using ShareLoc.Client.BL;
using ShareLoc.Client.BL.Services;

namespace ShareLoc.Client.App.ViewModels;

public sealed partial class PlaceDetailPageViewModel : BaseViewModel
{
	private readonly ModelMapper _modelMapper;
	private readonly IAlertService _alertService;
	private readonly ApiClient _apiClient;
	private readonly LocalDbService _localDbService;
	private readonly IOptions<ServerOptions> _serverOptions;
	private readonly INavigationService _navigationService;
	private readonly IMediator _mediator;

	public PlaceDetailViewModel PlaceDetailViewModel { get; }
	public PlaceModel? PlaceModel
	{
		get => PlaceDetailViewModel.Model;
		set => PlaceDetailViewModel.Model = value;
	}

	public PlaceDetailPageViewModel(PlaceDetailViewModel placeDetailViewModel, ModelMapper modelMapper, IAlertService alertService, ApiClient apiClient, LocalDbService localDbService, IOptions<ServerOptions> serverOptions, INavigationService navigationService, IMediator mediator)
	{
		PlaceDetailViewModel = placeDetailViewModel;
		_modelMapper = modelMapper;
		_alertService = alertService;
		_apiClient = apiClient;
		_localDbService = localDbService;
		_serverOptions = serverOptions;
		_navigationService = navigationService;
		_mediator = mediator;
	}


	[RelayCommand]
	private async Task Delete(CancellationToken ct)
	{
		if (PlaceDetailViewModel.Model is null)
			return;

		var confirm = await _alertService.ShowConfirmationAsync("Delete", "Do you want to delete this place?", "YES", "NO");
		if (!confirm)
			return;

		await _localDbService.DeleteAsync(PlaceDetailViewModel.Model.LocalId, ct);

		_mediator.Publish(new PlaceDeletedMessage(PlaceDetailViewModel.Model.LocalId));
		await _navigationService.GoBackAsync();
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
			},
			async validationErrors => await _alertService.ShowAlertAsync("Error", $"Invalid values given: {validationErrors}", "OK"),
			async error => await _alertService.ShowAlertAsync("Error", $"Error while creating place: {error}", "OK")
		);
	}
}
