using Microsoft.Extensions.DependencyInjection;

using ShareLoc.Server.DAL.Repositories;
using ShareLoc.Server.DAL.Services;

namespace ShareLoc.Server.DAL.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDAL(this IServiceCollection services)
	{
		services.AddSingleton<PlaceRepository>();
		services.AddSingleton<PlaceService>();

		return services;
	}
}
