using CommunityToolkit.Mvvm.Input;

using ShareLoc.Client.App.Services;
using ShareLoc.Client.App.Views.Pages;

namespace ShareLoc.Client.App.ViewModels;

public sealed partial class MainMenuPageViewModel : BaseViewModel
{
	private readonly INavigationService _navigationService;

	public string AppVersion => AppInfo.Current.VersionString;

	public MainMenuPageViewModel(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}

	[RelayCommand]
	public Task GoToCreatePlacePage() => _navigationService.GoToAsync<CreatePlacePage, CreatePlaceViewModel>();

	[RelayCommand]
	public Task GoToMyPlacesPage() => _navigationService.GoToAsync<MyPlacesPage, MyPlacesPageViewModel>();

	[RelayCommand]
	public Task GoToMyGuessesPage() => _navigationService.GoToAsync<MyGuessesPage, MyGuessesViewModel>();
}
