using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using ShareLoc.Client.App.Extensions;
using ShareLoc.Client.BL;
using ShareLoc.Client.BL.Extensions;
using ShareLoc.Shared.Common;

namespace ShareLoc.Client.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder.Configuration
			.AddJsonFile("appsettings.json");

		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddOptions<ServerOptions>()
			.Bind(builder.Configuration.GetRequiredSection(ServerOptions.SectionName));

		builder.Services
			.AddCommon()
			.AddBL()
			.AddViewsAndViewModels()
			.AddServices();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
