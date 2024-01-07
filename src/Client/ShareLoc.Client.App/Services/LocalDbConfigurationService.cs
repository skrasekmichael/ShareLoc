using ShareLoc.Client.DAL;

namespace ShareLoc.Client.App.Services;

public sealed class LocalDbConfigurationService : ILocalDbConfigurationService
{
	private const string LiteDbFileName = "ShareLoc.db";

	public string GetFilePath() => Path.Combine(FileSystem.AppDataDirectory, LiteDbFileName);
}
