using ShareLoc.Client.App.ViewModels;
using ShareLoc.Client.App.Views.Pages;

namespace ShareLoc.Client.App.Services;

public sealed class OnStartUpService
{
	private readonly IServiceProvider _serviceProvider;

	private Page? _scheduledPage;

	public OnStartUpService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public void ScheduleGuessingPage(Uri uri)
	{
		var guessingPage = _serviceProvider.GetRequiredService<GuessingPage>();
		var guessingPageViewModel = _serviceProvider.GetRequiredService<GuessingPageViewModel>();

		var uriBuilder = new UriBuilder(uri)
		{
			Scheme = "https"
		};

		guessingPageViewModel.Uri = uriBuilder.Uri;

		guessingPage.BindingContext = guessingPageViewModel;
		_scheduledPage = guessingPage;

		guessingPageViewModel.LoadAsynchronously();
	}

	public Page GetInitialPage()
	{
		return _scheduledPage switch
		{
			null => _serviceProvider.GetRequiredService<MainMenuPage>(),
			_ => _scheduledPage
		};
	}
}
