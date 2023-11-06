using ShareLoc.Client.App.ViewModels;

namespace ShareLoc.Client.App.Views.Pages;

public sealed partial class MainMenuPage : ContentPage
{
	public MainMenuPage(MainMenuViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

