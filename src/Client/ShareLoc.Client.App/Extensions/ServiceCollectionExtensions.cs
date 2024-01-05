using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.Messaging;

using ShareLoc.Client.App.Services;
using ShareLoc.Client.App.ViewModels;
using ShareLoc.Client.App.Views;
using ShareLoc.Client.App.Views.Pages;
using ShareLoc.Client.DAL;

namespace ShareLoc.Client.App.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddViewsAndViewModels(this IServiceCollection services)
	{
		//pages
		services.AddScoped<MainMenuPage, MainMenuPageViewModel>();
		services.AddScoped<MyPlacesPage, MyPlacesPageViewModel>();
		services.AddScoped<MyGuessesPage, MyGuessesViewModel>();
		services.AddScoped<PlaceDetailPage, PlaceDetailPageViewModel>();
		services.AddTransient<CreatePlacePage, CreatePlaceViewModel>();

		//views
		services.AddTransient<PlaceDetailView, PlaceDetailViewModel>();
		return services;
	}

	public static IServiceCollection AddServices(this IServiceCollection services)
	{
		return services
			.AddSingleton<INavigationService, NavigationService>()
			.AddSingleton<ILocalDbConfigurationService, LocalDbConfigurationService>()
			.AddSingleton<IAlertService, AlertService>()
			.AddSingleton<ModelMapper>()
			.AddSingleton<IMediator>(new Mediator(WeakReferenceMessenger.Default))
			.AddSingleton<PlaceSharingService>();
	}
}
