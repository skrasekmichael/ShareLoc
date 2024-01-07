using ShareLoc.Client.App.Views.Pages;
using ShareLoc.Client.DAL;

namespace ShareLoc.Client.App;

public sealed partial class App : Application
{
	private readonly ApplicationDbContext _dbContext;

	public App(IServiceProvider serviceProvider, ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;

		InitializeComponent();

		var mainMenuPage = serviceProvider.GetRequiredService<MainMenuPage>();
		MainPage = new NavigationPage(mainMenuPage);
	}

	protected override async void OnStart()
	{
		try
		{
			_dbContext.Initialize();
		}
		catch (Exception ex)
		{
			await MainPage!.DisplayAlert("Critical Error!", ex.InnerException?.Message ?? ex.Message, "OK");
			Current?.Quit();
		}
	}
}
