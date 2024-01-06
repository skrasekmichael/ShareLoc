using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

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

	public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration config)
	{
		const int defaultValue = -1;

		string policy = config["RateLimiting:Policy"] ?? throw new Exception("Missing rate limiting configuration.");

		int tokenLimit = config.GetValue("RateLimiting:TokenLimit", defaultValue);
		int replenishmentPeriodSeconds = config.GetValue("RateLimiting:ReplenishmentPeriodSeconds", defaultValue);
		int tokensPerPeriod = config.GetValue("RateLimiting:TokensPerPeriod", defaultValue);
		int queueLimit = config.GetValue("RateLimiting:QueueLimit", defaultValue);

		if (tokenLimit == defaultValue || replenishmentPeriodSeconds == defaultValue || tokensPerPeriod == defaultValue || queueLimit == defaultValue)
			throw new Exception("Missing rate limiting configuration.");

		services.AddRateLimiter(_ => _
			.AddTokenBucketLimiter(policyName: policy, options =>
			{
				options.TokenLimit = tokenLimit;
				options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
				options.QueueLimit = queueLimit;
				options.ReplenishmentPeriod = TimeSpan.FromSeconds(replenishmentPeriodSeconds);
				options.TokensPerPeriod = tokensPerPeriod;
				options.AutoReplenishment = true;
			}));

		return services;
	}
}
