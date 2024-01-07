using CommunityToolkit.Maui;

#if DEBUG
using Microsoft.Extensions.Logging;
#endif

using ShareLoc.Client.App.Extensions;
using ShareLoc.Client.BL.Extensions;
using ShareLoc.Client.DAL;
using ShareLoc.Shared.Common;

namespace ShareLoc.Client.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services
			.AddCommon()
			.AddDAL()
			.AddBL()
			.AddViewsAndViewModels()
			.AddServices();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
