using Amazon.DynamoDBv2;

using ShareLoc.Server.DAL.Helpers;

namespace ShareLoc.Server.App.Extensions;

public static class ServiceProviderExtensions
{
	public static async Task EnsureTablesCreatedAsync(this IServiceProvider serviceProvider, CancellationToken ct = default)
	{
		var client = serviceProvider.GetRequiredService<IAmazonDynamoDB>();

		var dbInitializer = new DbInitializer(client);
		await dbInitializer.CreateTablesAsync(ct);
	}
}
