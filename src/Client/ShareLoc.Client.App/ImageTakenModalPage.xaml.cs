namespace ShareLoc.Client.App;

public partial class ImageTakenModalPage : ContentPage
{
	public ImageTakenModalPage(ImageSource image)
	{
		InitializeComponent();
		ImageComponent.Source = image;
	}
}
