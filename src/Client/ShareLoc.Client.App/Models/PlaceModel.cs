using CommunityToolkit.Mvvm.ComponentModel;

namespace ShareLoc.Client.App.Models;

public sealed partial class PlaceModel : ObservableObject
{
	public Guid LocalId { get; set; }

	public Guid ServerId { get; set; }
	public double Latitude { get; set; }
	public double Longitude { get; set; }
	public byte[] Image { get; set; } = [];
	public string Message { get; set; } = "Guess Where I am!";
	public DateTime CreatedUTC { get; set; }
	public DateTime SharedUTC { get; set; }

	public bool IsShared => ServerId != Guid.Empty;
}
