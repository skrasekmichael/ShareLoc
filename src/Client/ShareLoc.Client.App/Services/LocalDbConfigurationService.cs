using ShareLoc.Client.DAL;

namespace ShareLoc.Client.App.Services;

public sealed class LocalDbConfigurationService : ILocalDbConfigurationService
{
	private const string SQLiteDbFileName = "ShareLoc.db3";

	public string GetConnectionString()
	{
		var path = Path.Combine(FileSystem.AppDataDirectory, SQLiteDbFileName);
		return $"Data Source={path}";
	}
}
