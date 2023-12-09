namespace ShareLoc.Client.App.Services;

public sealed class AlertService : IAlertService
{
	private readonly INavigationService _navigationService;

	public AlertService(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}

	public Task ShowAlertAsync(string title, string message, string cancel)
		=> _navigationService.GetCurrentPage().DisplayAlert(title, message, cancel);

	public Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
		=> _navigationService.GetCurrentPage().DisplayAlert(title, message, accept, cancel);
}
