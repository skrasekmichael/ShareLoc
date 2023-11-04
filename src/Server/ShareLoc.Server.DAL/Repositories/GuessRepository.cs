using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

using ShareLoc.Server.DAL.Entities;

namespace ShareLoc.Server.DAL.Repositories;

public sealed class GuessRepository : RepositoryBase
{
	public GuessRepository(DynamoDBContext dbContext) : base(dbContext) { }

	public Task InsertGuessAsync(Guess guess, CancellationToken token = default) =>
		_dbContext.SaveAsync(guess, token);

	public Task<Guess?> GetGuessByIdAsync(Guid id, CancellationToken token = default) =>
		_dbContext.LoadAsync<Guess?>(id, token);

	public Task<List<Guess>> GetGuessesByGuesserIdAsync(Guid guesserId, CancellationToken token = default)
	{
		List<ScanCondition> scanConditions = new()
		{
			new ScanCondition("GuesserId", ScanOperator.Equal, guesserId)
		};

		return _dbContext.ScanAsync<Guess>(scanConditions).GetRemainingAsync();
	}
}
