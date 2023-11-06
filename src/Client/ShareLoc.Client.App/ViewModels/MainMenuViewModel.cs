using CommunityToolkit.Mvvm.Input;

using ShareLoc.Client.App.Services;
using ShareLoc.Client.App.Views.Pages;

namespace ShareLoc.Client.App.ViewModels;

public sealed partial class MainMenuViewModel : BaseViewModel
{
	private readonly INavigationService _navigationService;

	public string AppVersion => AppInfo.Current.VersionString;

	public MainMenuViewModel(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}

	[RelayCommand]
	public Task GoToCreatePlacePage()
	{
		//TODO
		return Task.CompletedTask;
	}

	[RelayCommand]
	public Task GoToMyPlacesPage() => _navigationService.GoToAsync<MyPlacesPage, MyPlacesViewModel>();

	[RelayCommand]
	public Task GoToMyGuessesPage() => _navigationService.GoToAsync<MyGuessesPage, MyGuessesViewModel>();
}
