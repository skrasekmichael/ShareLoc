using Amazon.DynamoDBv2.DataModel;

using ShareLoc.Server.DAL.Entities;

namespace ShareLoc.Server.DAL.Repositories;

public sealed class PlaceRepository : RepositoryBase
{
	public PlaceRepository(IDynamoDBContext dbContext) : base(dbContext) { }

	public Task InsertPlaceAsync(Place place, CancellationToken token = default) =>
		_dbContext.SaveAsync(place, token);

	public Task<Place?> GetPlaceByIdAsync(Guid id, CancellationToken token = default) =>
		_dbContext.LoadAsync<Place?>(id, token);
}
