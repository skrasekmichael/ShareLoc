using ShareLoc.Client.App.ViewModels;

namespace ShareLoc.Client.App.Services;

public interface INavigationService
{
	Task GoToAsync(Page page, bool animated = true);
	Task GoToAsync<TPage, TViewModel>(TViewModel viewModel, bool animated = true) where TPage : Page where TViewModel : BaseViewModel;
	Task GoToAsync<TPage, TViewModel>(bool animated = true) where TPage : Page where TViewModel : BaseViewModel;
	Page GetCurrentPage();
	Task ReturnToRootAsync(bool animated = true);
}
