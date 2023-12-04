using CommunityToolkit.Mvvm.ComponentModel;

using ShareLoc.Client.DAL.Entities;

namespace ShareLoc.Client.App.Models;

public sealed partial class PlaceModel : ObservableObject
{
	private readonly PlaceEntity _placeEntity;

	public PlaceModel(PlaceEntity placeEntity)
	{
		_placeEntity = placeEntity;
	}

	public Guid LocalId => _placeEntity.LocalId;

	public byte[] Image => _placeEntity.Image;

	[ObservableProperty]
	private bool _isSelected = false;

	public DateTime CreatedUTC => _placeEntity.CratedUTC;

	public bool IsShared => _placeEntity.IsShared;

	public double Latitude => _placeEntity.Latitude;

	public double Longitude => _placeEntity.Longitude;

	public string Message => _placeEntity.Message;
}
