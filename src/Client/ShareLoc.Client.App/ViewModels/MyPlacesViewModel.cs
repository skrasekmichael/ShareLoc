using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;

using ShareLoc.Client.App.Messages;
using ShareLoc.Client.App.Models;
using ShareLoc.Client.App.Services;
using ShareLoc.Client.App.Views.Pages;
using ShareLoc.Client.BL.Services;

namespace ShareLoc.Client.App.ViewModels;

public sealed partial class MyPlacesPageViewModel : BaseViewModel
{
	private readonly LocalDbService _dbService;
	private readonly INavigationService _navigationService;
	private readonly PlaceDetailPageViewModel _placeDetailViewModel;
	private readonly ModelMapper _modelMapper;
	private readonly IMediator _mediator;

	public ObservableCollection<PlaceModel> MyPlaces { get; } = [];

	public MyPlacesPageViewModel(INavigationService navigationService, LocalDbService dbService, PlaceDetailPageViewModel placeDetailViewModel, ModelMapper modelMapper, IMediator mediator)
	{
		_dbService = dbService;
		_navigationService = navigationService;
		_placeDetailViewModel = placeDetailViewModel;
		_modelMapper = modelMapper;
		_mediator = mediator;

		mediator.Subscribe<MyPlacesPageViewModel, PlaceStoredMessage>(this, (recipient, message) => recipient.MyPlaces.Add(message.Model));
		mediator.Subscribe<MyPlacesPageViewModel, PlaceDeletedMessage>(this, (recipient, message) =>
		{
			var deletedPlace = recipient.MyPlaces.First(place => place.LocalId == message.PlaceId);
			recipient.MyPlaces.Remove(deletedPlace);
		});
	}

	protected override async Task LoadAsync(CancellationToken ct)
	{
		var places = (await _dbService.GetPlacesAsync(ct))
			.Select(entity => _modelMapper.Map(entity))
			.ToList();

		await Dispatch(() => places.ForEach(MyPlaces.Add));
	}

	[RelayCommand]
	private Task TapPlace(PlaceModel place)
	{
		_placeDetailViewModel.PlaceModel = place;
		return _navigationService.GoToAsync<PlaceDetailPage, PlaceDetailPageViewModel>(_placeDetailViewModel);
	}
}
