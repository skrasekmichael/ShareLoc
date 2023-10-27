using Amazon.DynamoDBv2.DataModel;

namespace ShareLoc.Server.DAL.Repositories;

public abstract class RepositoryBase
{
	protected readonly DynamoDBContext _dbContext;

	public RepositoryBase(DynamoDBContext dbContext)
	{
		_dbContext = dbContext;
	}
}
