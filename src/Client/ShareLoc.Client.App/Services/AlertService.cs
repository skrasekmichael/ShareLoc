using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLoc.Client.App.Services;

class AlertService : IAlertService
{
	private readonly INavigationService _navigationService;
	public AlertService(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}

	public async Task ShowAlertAsync(string title, string message, string cancel)
	{
		await _navigationService.GetCurrentPage().DisplayAlert(title, message, cancel);
	}

	public async Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
	{
		return await _navigationService.GetCurrentPage().DisplayAlert(title, message, accept, cancel);
	}
}
