using LiteDB;

using ShareLoc.Client.DAL.Entities;

namespace ShareLoc.Client.DAL;

public sealed class ApplicationDbContext
{
	private readonly ILocalDbConfigurationService _configurationService;

	public LiteDatabase Database { get; private set; } = default!;

	public ILiteCollection<PlaceEntity> Places => Database.GetCollection<PlaceEntity>("places");

	public ApplicationDbContext(ILocalDbConfigurationService configurationService)
	{
		_configurationService = configurationService;
	}

	public void Initialize()
	{
		Database = new(_configurationService.GetFilePath());

		var collection = Database.GetCollection<PlaceEntity>("places");
		collection.EnsureIndex(place => place.LocalId);
	}
}
