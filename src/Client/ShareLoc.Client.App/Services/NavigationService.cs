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

	public async Task GoToAsync<TPage, TViewModel>(TViewModel viewModel, bool animated = true) where TPage : Page where TViewModel : BaseViewModel
	{
		var page = _serviceProvider.GetRequiredService<TPage>();
		page.BindingContext = viewModel;
		await Navigation.PushAsync(page, animated);
		viewModel.LoadAsynchronously();
	}

	public async Task GoToAsync<TPage, TViewModel>(bool animated = true) where TPage : Page where TViewModel : BaseViewModel
	{
		var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
		var page = _serviceProvider.GetRequiredService<TPage>();
		page.BindingContext = viewModel;

		await GoToAsync(page, animated);
		viewModel.LoadAsynchronously();
	}
	public Page GetCurrentPage() => Navigation.NavigationStack.LastOrDefault() ?? throw new Exception("Could not get current page");
	public Task ReturnToRootAsync(bool animated = true) => Navigation.PopToRootAsync(animated);
}
