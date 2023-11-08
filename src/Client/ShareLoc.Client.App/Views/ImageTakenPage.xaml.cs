using System.Diagnostics;
using System.Reflection;

using ShareLoc.Client.App.ViewModels;

namespace ShareLoc.Client.App.Views;

[QueryProperty(nameof(ImageFile), "ImageFile")]
public partial class ImageTakenPage : ContentPage
{
	private FileResult _imageFile;
	public FileResult ImageFile
	{
		get
		{
			Debug.WriteLine($"Image file read: {ImageFile}");
			return _imageFile;
		}
		set
		{
			Debug.WriteLine($"Image file written: {value}");
			_imageFile = value;
			BindingContext = new ImageTakenViewModel(_imageFile);
		}
	}

	public ImageTakenPage()
	{
		InitializeComponent();
	}
}
