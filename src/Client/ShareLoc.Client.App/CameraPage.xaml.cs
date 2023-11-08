using System;
using System.Diagnostics;
using Microsoft.Maui.Devices.Sensors;
using Camera.MAUI;
using CommunityToolkit.Maui.Views;

namespace ShareLoc.Client.App;

public partial class CameraPage : ContentPage
{
	public CameraPage()
	{
		InitializeComponent();
	}

	private void CameraView_CamerasLoaded(object sender, EventArgs e)
	{
		cameraView.Camera = cameraView.Cameras.First();
		MainThread.BeginInvokeOnMainThread(async () =>
		{
			await cameraView.StopCameraAsync();
			// find the largest 4/3 resolution available or take first one if none available
			var resolution = cameraView.Camera.AvailableResolutions.Capacity > 0 ?
				cameraView.Camera.AvailableResolutions.OrderByDescending(x => x.Width * x.Height).FirstOrDefault(x => x.Width / x.Height == 4 / 3) :
				cameraView.Camera.AvailableResolutions.FirstOrDefault();
			var result = await cameraView.StartCameraAsync(resolution);
			Debug.WriteLine($"Camera resolution: {resolution.Width}x{resolution.Height}");
			if (result == CameraResult.Success)
			{
				cameraView.WidthRequest = this.Width;
				cameraView.HeightRequest = this.Width * 4 / 3;
				//cameraView.HeightRequest = cameraView.Height;

				ZoomSlider.Maximum = cameraView.MaxZoomFactor;
				ZoomSlider.Minimum = cameraView.MinZoomFactor;
			}
		});
	}

	private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
	{
		Debug.WriteLine($"Slider value changed: {e.NewValue}");
		//cameraView.ZoomFactor = (float)e.NewValue;
		Debug.WriteLine($"Camera zoom factor: {cameraView.ZoomFactor}");
	}

	private async void Button_Clicked(object sender, EventArgs e)
	{
		Debug.WriteLine("Button clicked");
		if (MediaPicker.Default.IsCaptureSupported)
		{
			Debug.WriteLine("Capture supported");
			FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
			Debug.WriteLine($"Photo: {photo}");
			if (photo != null)
			{
				// set the photo as image source
				byte[] imageData;
				string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
				Debug.WriteLine($"Local file path: {localFilePath}");
				using (Stream stream = await photo.OpenReadAsync())
				{
					Debug.WriteLine($"Stream opened");
					this.ShowPopup(new ImageTakenPopup(ImageSource.FromStream(() => stream)));
				}
				using (FileStream localFileStream = File.OpenWrite(localFilePath))
				{
					imageData = File.ReadAllBytes(localFilePath);
				}
				File.Delete(localFilePath);

				return;
			}
		}
		else
		{
			Debug.WriteLine("Capture not supported on this device");
		}

		var x = cameraView.GetSnapShot(Camera.MAUI.ImageFormat.PNG);
		this.ShowPopup(new ImageTakenPopup(x));
		GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
		var location = await Geolocation.GetLocationAsync(request);
		Debug.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
	}

	private void Slider_Loaded(object sender, EventArgs e)
	{
		Debug.WriteLine("Slider loaded");
		//zoomSlider.Maximum = cameraView.MaxZoomFactor;
		//zoomSlider.Minimum = cameraView.MinZoomFactor;
	}
}

