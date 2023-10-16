using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;

using ShareLoc.Server.App.Endpoints;
using ShareLoc.Server.App.Pages;

namespace ShareLoc.Server.App.Extensions;

public static class WebApplicationExtensions
{
	public static WebApplication MapEndpoints<TEndpoints>(this WebApplication app) where TEndpoints : IEndpoints, new()
	{
		var endpoint = new TEndpoints();
		endpoint.AddEndpoints(app);
		return app;
	}

	public static IServiceCollection AddDBContext(this IServiceCollection services, string serviceURL, string authRegion)
	{
		AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
		clientConfig.ServiceURL = serviceURL;
		clientConfig.AuthenticationRegion = authRegion;

		AmazonDynamoDBClient client = new AmazonDynamoDBClient(clientConfig);
		services.AddSingleton<IAmazonDynamoDB>(client);
		services.AddSingleton<IDynamoDBContext, DynamoDBContext>();

		return services;
	}

	public static WebApplication MapPage<TPage>(this WebApplication app) where TPage : Page, new()
	{
		var page = new TPage();
		page.Init();
		page.MapEndpoints(app);
		return app;
	}
}
