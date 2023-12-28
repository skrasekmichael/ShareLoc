using ShareLoc.Client.App.Services;

namespace ShareLoc.Client.App;

public sealed partial class App : Application
{
	public App(OnStartUpService onStartUpService)
	{
		InitializeComponent();
		MainPage = new NavigationPage(onStartUpService.GetInitialPage());
	}
}
