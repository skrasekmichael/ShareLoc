using ShareLoc.Server.DAL.Entities;
using ShareLoc.Server.DAL.Repositories;
using ShareLoc.Shared.Common.Models;

namespace ShareLoc.Server.DAL.Services;

public sealed class PlaceService
{
	private readonly PlaceRepository _placeRepository;

	public PlaceService(PlaceRepository placeRepository)
	{
		_placeRepository = placeRepository;
	}

	public async Task<Guid> CreatePlaceAsync(PlaceRequest request)
	{
		Place newPlace = new Place()
		{
			Id = Guid.NewGuid(),
			Latitude = request.Latitude,
			Longitude = request.Longitude,
			Message = request.Message,
			Image = request.Image,
			TimeStampUTC = DateTime.UtcNow
		};

		await _placeRepository.InsertPlaceAsync(newPlace);

		return newPlace.Id;
	}
}
