using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

using ShareLoc.Server.DAL.Entities;

namespace ShareLoc.Server.DAL.Repositories;

public sealed class PlaceRepository : RepositoryBase
{
	public PlaceRepository(IDynamoDBContext dbContext) : base(dbContext) { }

	public Task InsertPlaceAsync(Place place, CancellationToken token = default) =>
		_dbContext.SaveAsync(place, token);

	public Task<Place?> GetPlaceByIdAsync(Guid id, CancellationToken token = default) =>
		_dbContext.LoadAsync<Place?>(id, token);

	public Task DeletePlaceAsync(Place place, CancellationToken token = default) =>
		_dbContext.DeleteAsync(place, token);

	public Task<List<Place>> GetOldEntriesAsync(string isoTimeStampCutoff, CancellationToken token = default)
	{
		return _dbContext.ScanAsync<Place>(
			new List<ScanCondition>
			{
				new("TimeStampUTC", ScanOperator.LessThanOrEqual, isoTimeStampCutoff)
			}).GetRemainingAsync(token);
	}

	public Task DeletePlacesAsync(IEnumerable<Place> places, CancellationToken token = default)
	{
		var batch = _dbContext.CreateBatchWrite<Place>();
		batch.AddDeleteItems(places);

		return batch.ExecuteAsync(token);
	}
}
