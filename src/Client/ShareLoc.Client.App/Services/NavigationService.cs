using System.Diagnostics;

using ShareLoc.Client.App.ViewModels;

namespace ShareLoc.Client.App.Services;

public sealed class NavigationService : INavigationService
{
	private readonly IServiceProvider _serviceProvider;

	private static INavigation Navigation
	{
		get
		{
			var navigation = Application.Current?.MainPage?.Navigation;
			if (navigation is not null)
				return navigation;

			if (Debugger.IsAttached)
				Debugger.Break();
			throw new Exception();
		}
	}

	public NavigationService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public Task GoToAsync(Page page, bool animated = true) => Navigation.PushAsync(page, animated);

	public Task GoToAsync<TPage, TViewModel>(TViewModel viewModel, bool animated = true) where TPage : Page where TViewModel : BaseViewModel
	{
		var page = _serviceProvider.GetRequiredService<TPage>();
		page.BindingContext = viewModel;
		return Navigation.PushAsync(page, animated);
	}

	public Task GoToAsync<TPage, TViewModel>(bool animated = true) where TPage : Page where TViewModel : BaseViewModel
	{
		var page = _serviceProvider.GetRequiredService<TPage>();
		var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
		page.BindingContext = viewModel;
		return GoToAsync(page, animated);
	}
}
