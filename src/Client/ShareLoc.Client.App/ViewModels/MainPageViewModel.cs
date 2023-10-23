using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using CommunityToolkit.Maui.Core.Views;

namespace ShareLoc.Client.App.ViewModels;
internal class MainPageViewModel
{
	public ICommand SharePhotoAndLocation { get; }
	public MainPageViewModel()
	{
		SharePhotoAndLocation = new Command(async () =>
		{
			if (!MediaPicker.IsCaptureSupported)
			{
				// Display error alert, abort
				await Shell.Current.DisplayAlert("Error", "Camera not supported", "OK");
				return;
			}
			// get photo using media picker
			var photo = await MediaPicker.CapturePhotoAsync();

			// get location using geolocation
			var location = await Geolocation.GetLocationAsync();

			// TODO: send photo and location to server using HttpClient
			await Shell.Current.DisplayAlert("Success", $"Photo and location: {location.Latitude}, {location.Longitude} sent", "OK");
		});
	}

}
