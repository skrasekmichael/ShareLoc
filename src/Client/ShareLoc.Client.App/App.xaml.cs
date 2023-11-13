using ShareLoc.Client.App.Views.Pages;

namespace ShareLoc.Client.App;

public sealed partial class App : Application
{
	public App(MainMenuPage mainMenuPage)
	{
		InitializeComponent();
		MainPage = new NavigationPage(mainMenuPage);
	}
}
