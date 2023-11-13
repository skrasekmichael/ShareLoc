using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;

namespace ShareLoc.Server.App.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDBContext(this IServiceCollection services, IConfiguration config)
	{
		string serviceURL = config["DynamoDB:ServiceURL"] ?? throw new Exception("Missing DynamoDB configuration.");
		string authRegion = config["DynamoDB:AuthenticationRegion"] ?? throw new Exception("Missing DynamoDB configuration.");
		string accessKey = config["DynamoDB:AccessKey"] ?? throw new Exception("Missing DynamoDB configuration.");
		string secretKey = config["DynamoDB:SecretKey"] ?? throw new Exception("Missing DynamoDB configuration.");

		var clientConfig = new AmazonDynamoDBConfig
		{
			ServiceURL = serviceURL,
			AuthenticationRegion = authRegion
		};

		var client = new AmazonDynamoDBClient(accessKey, secretKey, clientConfig);
		services.AddSingleton<IAmazonDynamoDB>(client);
		services.AddSingleton<IDynamoDBContext, DynamoDBContext>();

		return services;
	}
}
