using ShareLoc.Client.App.Services;
using ShareLoc.Client.App.ViewModels;
using ShareLoc.Client.App.Views.Pages;

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
		services.AddSingleton<INavigationService, NavigationService>();

		return services;
	}
}
