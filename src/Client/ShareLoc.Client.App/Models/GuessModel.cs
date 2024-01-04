using CommunityToolkit.Mvvm.ComponentModel;

namespace ShareLoc.Client.App.Models;

public sealed partial class GuessModel : ObservableObject
{
	public required Guid GuesserId { get; init; }
	public required double Latitude { get; init; }
	public required double Longitude { get; init; }
	public required string Name { get; init; }
	public required int Score { get; init; }
	public required double Distance { get; init; }

	[ObservableProperty]
	private bool _isSelected;
}
