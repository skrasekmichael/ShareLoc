using Microsoft.EntityFrameworkCore;

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

	public async Task<OneOf<Success<Guid>, NotFound, Error<string>>> SavePlaceAsync(PlaceRequest place, CancellationToken ct = default)
	{
		try
		{
			Guid localId = Guid.NewGuid();
			await _dbContext.Places.AddAsync(new()
			{
				LocalId = localId,
				CratedUTC = DateTime.UtcNow,
				Image = place.Image,
				Latitude = place.Latitude,
				Longitude = place.Longitude,
				Message = place.Message,
				ServerId = Guid.Empty,
				SharedUTC = DateTime.MinValue
			}, ct);
			await _dbContext.SaveChangesAsync(ct);
			return new Success<Guid>(localId);
		}
		catch (Exception ex)
		{
			return new Error<string>(ex.Message);
		}
	}

	public async Task<OneOf<Success, NotFound, Error<string>>> SharePlaceAsync(Guid localPlaceId, Guid serverPlaceId, DateTime sharedUTC, CancellationToken ct = default)
	{
		var place = await _dbContext.Places.FindAsync([localPlaceId], cancellationToken: ct);

		if (place is null)
			return new NotFound();

		if (place.IsShared)
			return new Error<string>("This place is already shared.");

		place.ServerId = serverPlaceId;
		place.SharedUTC = sharedUTC;

		try
		{
			await _dbContext.SaveChangesAsync(ct);
			return new Success();
		}
		catch (Exception ex)
		{
			return new Error<string>(ex.Message);
		}
	}

	public async Task<OneOf<Success, NotFound, Error<string>>> UpdateMessageAsync(Guid localPlaceId, string message, CancellationToken ct = default)
	{
		var place = await _dbContext.Places.FindAsync([localPlaceId], cancellationToken: ct);

		if (place is null)
			return new NotFound();

		if (place.IsShared)
			return new Error<string>("Can't update shared place.");

		place.Message = message;

		try
		{
			await _dbContext.SaveChangesAsync(ct);
			return new Success();
		}
		catch (Exception ex)
		{
			return new Error<string>(ex.Message);
		}
	}

	public Task<List<PlaceEntity>> GetPlacesAsync(CancellationToken ct = default) =>
		_dbContext.Places.Include(place => place.Guesses).ToListAsync(ct);
}
