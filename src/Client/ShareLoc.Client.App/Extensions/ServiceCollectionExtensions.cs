using CommunityToolkit.Maui;

using ShareLoc.Client.App.Services;
using ShareLoc.Client.App.ViewModels;
using ShareLoc.Client.App.Views.Pages;
using ShareLoc.Client.DAL;

namespace ShareLoc.Client.App.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddViewsAndViewModels(this IServiceCollection services)
	{
		services.AddScoped<MainMenuPage, MainMenuViewModel>();
		services.AddScoped<MyPlacesPage, MyPlacesViewModel>();
		services.AddScoped<MyGuessesPage, MyGuessesViewModel>();
		services.AddScoped<PlaceDetailPage, PlaceDetailViewModel>();

		return services;
	}

	public static IServiceCollection AddServices(this IServiceCollection services)
	{
		return services
			.AddSingleton<INavigationService, NavigationService>()
			.AddSingleton<ILocalDbConfigurationService, LocalDbConfigurationService>()
			.AddSingleton<IAlertService, AlertService>();
	}
}
