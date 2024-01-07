using Microsoft.Extensions.DependencyInjection;

using ShareLoc.Client.BL.Services;

namespace ShareLoc.Client.BL.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddBL(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddHttpClient<ApiClient>("ServerHttpClient")
			//#if DEBUG
			.ConfigurePrimaryHttpMessageHandler(() =>
			{
				var handler = new HttpClientHandler
				{
					// Ignore SSL certificate validation errors
					ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
				};

				return handler;
			})
			//#endif
			.ConfigureHttpClient((serviceProvider, httpClient) =>
			{
				var options = serviceProvider.GetRequiredService<ServerOptions>();
				httpClient.BaseAddress = new Uri(options.Address);
			});

		return serviceCollection
			.AddSingleton<EntityMapper>()
			.AddSingleton(new ImageDownScaler(400 * 1024, [1.0, 0.5, 0.25, 0.18]))
			.AddScoped<LocalDbService>()
			.AddSingleton<ServerOptions>();
	}
}
