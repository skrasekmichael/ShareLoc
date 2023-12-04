using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ShareLoc.Client.App.Models;
using ShareLoc.Client.App.Services;
using ShareLoc.Client.App.Views.Pages;
using ShareLoc.Client.BL.Services;

namespace ShareLoc.Client.App.ViewModels;

public sealed partial class MyPlacesViewModel : BaseViewModel
{
	private readonly LocalDbService _dbService;
	private readonly INavigationService _navigationService;
	private readonly PlaceDetailViewModel _placeDetailViewModel;

	[ObservableProperty]
	private List<PlaceModel> _myPlaces = [];

	public MyPlacesViewModel(INavigationService navigationService, LocalDbService dbService, PlaceDetailViewModel placeDetailViewModel)
	{
		_dbService = dbService;
		_navigationService = navigationService;
		_placeDetailViewModel = placeDetailViewModel;
	}

	protected override async Task LoadAsync(CancellationToken ct)
	{
		var places = (await _dbService.GetPlacesAsync(ct))
			.Select(entity => new PlaceModel(entity))
			.ToList();

		MyPlaces = places;
	}

	[RelayCommand]
	private Task TapPlace(PlaceModel place)
	{
		_placeDetailViewModel.PlaceModel = place;
		return _navigationService.GoToAsync<PlaceDetailPage, PlaceDetailViewModel>(_placeDetailViewModel);
	}
}
