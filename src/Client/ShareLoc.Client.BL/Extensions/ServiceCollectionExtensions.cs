using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using ShareLoc.Client.BL.Services;

namespace ShareLoc.Client.BL.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddBL(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddHttpClient<ApiClient>("ServerHttpClient", (serviceProvider, httpClient) =>
		{
			var serverOptions = serviceProvider.GetRequiredService<IOptions<ServerOptions>>();
			httpClient.BaseAddress = new Uri(serverOptions.Value.Address);
		});

		return serviceCollection
			.AddSingleton<EntityMapper>()
			.AddSingleton(new ImageDownScaler(400 * 1024, [1.0, 0.5, 0.25, 0.18]))
			.AddScoped<LocalDbService>();
	}
}
