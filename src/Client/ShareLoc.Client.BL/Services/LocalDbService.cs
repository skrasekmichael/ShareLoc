using OneOf;
using OneOf.Types;

using ShareLoc.Client.DAL;
using ShareLoc.Client.DAL.Entities;
using ShareLoc.Shared.Common.Models;

namespace ShareLoc.Client.BL.Services;

public sealed class LocalDbService
{
	private readonly ApplicationDbContext _dbContext;

	public LocalDbService(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public OneOf<Success<PlaceEntity>, Error<string>> SavePlace(PlaceRequest place)
	{
		var localId = Guid.NewGuid();
		var entity = new PlaceEntity
		{
			LocalId = localId,
			CreatedUTC = DateTime.UtcNow,
			Image = place.Image,
			Latitude = place.Latitude,
			Longitude = place.Longitude,
			Message = place.Message,
			ServerId = Guid.Empty,
			SharedUTC = DateTime.MinValue
		};

		try
		{
			_dbContext.Places.Insert(entity);
			return new Success<PlaceEntity>(entity);
		}
		catch (Exception ex)
		{
			return new Error<string>(ex.Message);
		}
	}

	public OneOf<Success, NotFound, Error<string>> SharePlace(Guid localPlaceId, Guid serverPlaceId, DateTime sharedUTC)
	{
		var place = _dbContext.Places.FindById(new(localPlaceId));

		if (place is null)
			return new NotFound();

		if (place.IsShared)
			return new Error<string>("This place is already shared.");

		place.ServerId = serverPlaceId;
		place.SharedUTC = sharedUTC;

		try
		{
			_dbContext.Places.Update(place);
			return new Success();
		}
		catch (Exception ex)
		{
			return new Error<string>(ex.Message);
		}
	}

	public OneOf<Success, NotFound, Error<string>> UpdateMessage(Guid localPlaceId, string message)
	{
		var place = _dbContext.Places.FindById(new(localPlaceId));

		if (place is null)
			return new NotFound();

		if (place.IsShared)
			return new Error<string>("Can't update shared place.");

		place.Message = message;

		try
		{
			_dbContext.Places.Update(place);
			return new Success();
		}
		catch (Exception ex)
		{
			return new Error<string>(ex.Message);
		}
	}

	public void Delete(Guid localPlaceId) => _dbContext.Places.Delete(new(localPlaceId));

	public void RemoveServerId(Guid localPlaceId)
	{
		var place = _dbContext.Places.FindById(new(localPlaceId));
		if (place is not null)
		{
			place.ServerId = Guid.Empty;
			_dbContext.Places.Update(place);
		}
	}

	public IEnumerable<PlaceEntity> GetPlaces() => _dbContext.Places.FindAll();
}
