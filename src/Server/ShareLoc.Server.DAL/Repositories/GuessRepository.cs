using Amazon.DynamoDBv2.DataModel;

using ShareLoc.Server.DAL.Entities;

namespace ShareLoc.Server.DAL.Repositories;

public sealed class GuessRepository : RepositoryBase
{
	public GuessRepository(DynamoDBContext dbContext) : base(dbContext) { }

	public Task InsertGuessAsync(Guess guess, CancellationToken token = default) =>
		_dbContext.SaveAsync(guess, token);

	public Task<Guess?> GetGuessByIdAsync(Guid id, CancellationToken token = default) =>
		_dbContext.LoadAsync<Guess?>(id, token);
}
