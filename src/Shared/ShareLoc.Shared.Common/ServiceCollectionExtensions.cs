using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

using ShareLoc.Shared.Common.Models;

namespace ShareLoc.Shared.Common;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCommon(this IServiceCollection services)
	{
		services.AddScoped<IValidator<CreatePlaceRequest>, CreatePlaceRequestValidator>();
		services.AddScoped<IValidator<CreateGuessRequest>, CreateGuessRequestValidator>();

		return services;
	}
}
