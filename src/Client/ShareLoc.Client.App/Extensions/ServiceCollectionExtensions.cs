using ShareLoc.Client.App.Services;
using ShareLoc.Client.App.ViewModels;
using ShareLoc.Client.App.Views.Pages;
using ShareLoc.Client.DAL;

namespace ShareLoc.Client.App.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddViewsAndViewModels(this IServiceCollection services)
	{
		services.AddTransient<MainMenuPage>();
		services.AddTransient<MainMenuViewModel>();

		services.AddTransient<MyPlacesPage>();
		services.AddTransient<MyPlacesViewModel>();

		services.AddTransient<MyGuessesPage>();
		services.AddTransient<MyGuessesViewModel>();

		return services;
	}

	public static IServiceCollection AddServices(this IServiceCollection services)
	{
		return services
			.AddSingleton<INavigationService, NavigationService>()
			.AddSingleton<ILocalDbConfigurationService, LocalDbConfigurationService>();
	}
}
