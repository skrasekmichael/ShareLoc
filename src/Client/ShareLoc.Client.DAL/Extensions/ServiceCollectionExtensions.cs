using Microsoft.Extensions.DependencyInjection;

namespace ShareLoc.Client.DAL;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDAL(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddSingleton<ApplicationDbContext>();

		return serviceCollection;
	}
}
