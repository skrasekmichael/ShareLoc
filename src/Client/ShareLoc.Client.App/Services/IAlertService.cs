using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLoc.Client.App.Services;

public interface IAlertService
{
	Task ShowAlertAsync(string title, string message, string cancel);
	Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel);
}
