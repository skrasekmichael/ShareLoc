using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ShareLoc.Client.DAL;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDAL(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddDbContext<ApplicationDbContext>((serviceProvider, config) =>
		{
			using var scope = serviceProvider.CreateScope();
			var options = scope.ServiceProvider.GetRequiredService<ILocalDbConfigurationService>();
			config.UseSqlite(options.GetConnectionString());
		});

		return serviceCollection;
	}
}
