using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShareLoc.Client.App.ViewModels;

internal sealed partial class MainMenuViewModel : ObservableObject
{
	public string AppVersion => AppInfo.Current.VersionString;

	[RelayCommand]
	public void GoToCreatePlacePage()
	{

	}

	[RelayCommand]
	public void GoToGalleryPage()
	{

	}

	[RelayCommand]
	public void GoToMyGuessesPage()
	{

	}
}
