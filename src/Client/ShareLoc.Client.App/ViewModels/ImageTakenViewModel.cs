using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShareLoc.Client.App.ViewModels;
// ImageTakenViewModel provides a property for the image taken by the camera.
internal class ImageTakenViewModel : ViewModelBase
{
	private ImageSource _imageSource;
	public ImageSource ImageSource
	{
		get => _imageSource;
		set => SetProperty(ref _imageSource, value);
	}

	public ICommand ShareImageCommand { get; }
	public ICommand CancelCommand { get; }

	public ImageTakenViewModel(FileResult imageFile)
	{
		// image file is set as the image source
		_imageSource = ImageSource.FromStream(() => imageFile.OpenReadAsync().Result);

		// The ImageTaken view is passed the image file from the camera.
		// The image file is then shared
		ShareImageCommand = new Command(async () =>
		{
			await ShareImage(imageFile);
		});
		// The user may instead choose to cancel the image sharing.
		CancelCommand = new Command(async () =>
		{
			await Cancel();
		});
	}

	private async Task ShareImage(FileResult imageFile)
	{
		Debug.WriteLine($"ShareImage: {imageFile.FullPath}");
		// The image file is shared with the database.
		// The image file is then shared with other users.

		// Await send image to web api

		// Await generated link to image

		await Share.RequestAsync(new ShareFileRequest
		{
			Title = "Share photo",
			File = new ShareFile(imageFile.FullPath)
		});

	}

	private async Task Cancel()
	{
		Debug.WriteLine("Cancel");
		// The user is returned to the main page.
		await Shell.Current.GoToAsync("..");
	}
}
