using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Views;

namespace ShareLoc.Client.App;

public partial class ImageTakenPopup : Popup
{
	public ImageTakenPopup(ImageSource image)
	{
		InitializeComponent();
		ImageComponent.Source = image;
	}
}
