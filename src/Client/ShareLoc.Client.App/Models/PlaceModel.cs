using CommunityToolkit.Mvvm.ComponentModel;

namespace ShareLoc.Client.App.Models;

public sealed partial class PlaceModel : ObservableObject
{
	public Guid LocalId { get; set; }

	private Guid _serverId;
	public Guid ServerId
	{
		get => _serverId;
		set
		{
			_serverId = value;
			OnPropertyChanged(nameof(IsShared));
		}
	}

	public double Latitude { get; set; }
	public double Longitude { get; set; }
	public byte[] Image { get; set; } = [];

	private string _message = "Guess Where I am!";
	public string Message
	{
		get => _message;
		set
		{
			if (_message != value)
			{
				_message = value;
				OnPropertyChanged(nameof(Message));
				IsModified = true;
			}
		}
	}

	public DateTime CreatedUTC { get; set; }
	public DateTime SharedUTC { get; set; }

	public bool IsShared => ServerId != Guid.Empty;

	[ObservableProperty]
	private bool _isModified = false;
}
