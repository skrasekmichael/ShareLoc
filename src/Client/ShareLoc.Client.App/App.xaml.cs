using ShareLoc.Client.App.Views.Pages;

namespace ShareLoc.Client.App;

public sealed partial class App : Application
{
	public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();

		var mainMenuPage = serviceProvider.GetRequiredService<MainMenuPage>();
		MainPage = new NavigationPage(mainMenuPage);
	}
}
