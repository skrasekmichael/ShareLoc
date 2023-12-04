using CommunityToolkit.Mvvm.ComponentModel;

using ShareLoc.Client.App.Models;

namespace ShareLoc.Client.App.ViewModels;

public sealed partial class PlaceDetailViewModel : BaseViewModel
{
	[ObservableProperty]
	private PlaceModel? _placeModel;
}
