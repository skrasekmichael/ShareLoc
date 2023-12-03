using ShareLoc.Client.App.Services;

namespace ShareLoc.Client.App.Views.Pages;

public sealed partial class CreatePlacePage : ContentPage
{
	private readonly INavigationService _navigationService;
	public CreatePlacePage(INavigationService navigationService)
	{
		InitializeComponent();

		_navigationService = navigationService;
		// Make Image expand to fill the whole screen on tap and collapse on tap again
		var tapGestureRecognizer = new TapGestureRecognizer();
		tapGestureRecognizer.Tapped += OnImageTapped!;
		photo.GestureRecognizers.Add(tapGestureRecognizer);
	}

	private void OnImageTapped(object sender, EventArgs e)
	{
		if (sender is not Image photo) return;

		var windowWidth = _navigationService.GetCurrentPage().Width;
		var mapHeight = map.Height;
		var scale = Math.Min(windowWidth / photo.Width, mapHeight / photo.Height);

		if (photo.Scale == 1)
		{
			photo.ScaleTo(scale * 0.8, 250, Easing.CubicInOut);
			photo.TranslateTo((windowWidth - photo.Width * 0.8) / 2, (mapHeight - photo.Height) / 2, 250, Easing.CubicInOut);
		}
		else
		{
			photo.ScaleTo(1, 250, Easing.CubicInOut);
			photo.TranslateTo(0, 0, 250, Easing.CubicInOut);
		}
	}
}
