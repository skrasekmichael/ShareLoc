using Amazon.DynamoDBv2.DataModel;

namespace ShareLoc.Server.DAL.Repositories;

public abstract class RepositoryBase
{
	protected readonly IDynamoDBContext _dbContext;

	public RepositoryBase(IDynamoDBContext dbContext)
	{
		_dbContext = dbContext;
	}
}
