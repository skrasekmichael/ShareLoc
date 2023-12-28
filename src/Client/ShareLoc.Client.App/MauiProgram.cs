using System.Reflection;

using CommunityToolkit.Maui;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

using ShareLoc.Client.App.Extensions;
using ShareLoc.Client.App.Services;
using ShareLoc.Client.BL;
using ShareLoc.Client.BL.Extensions;
using ShareLoc.Client.DAL;
using ShareLoc.Shared.Common;

namespace ShareLoc.Client.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		var appSettingsStream = Assembly.GetExecutingAssembly()
			.GetManifestResourceStream("AppSettings") ?? throw new ArgumentNullException();

		builder.Configuration.AddJsonStream(appSettingsStream);

		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.ConfigureLifecycleEvents(lifecycle =>
			{
#if WINDOWS
				lifecycle.AddWindows(windows => windows.OnLaunching((_, _) =>
				{
					var activationArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
					if (activationArgs.Kind == Microsoft.Windows.AppLifecycle.ExtendedActivationKind.Protocol)
					{
						if (activationArgs.Data is not Windows.ApplicationModel.Activation.ProtocolActivatedEventArgs protocolArgs)
							return;

						var startupService = IPlatformApplication.Current?.Services.GetRequiredService<OnStartUpService>();
						startupService?.ScheduleGuessingPage(protocolArgs.Uri);
					}
				}));
#elif ANDROID
				lifecycle.AddAndroid(android => android.OnCreate((activity, _) =>
				{
					var schema = activity?.Intent?.Data?.EncodedSchemeSpecificPart;
					if (schema == "app-shareloc")
					{
						var uri = new Uri(activity?.Intent?.Data?.Path ?? throw new ArgumentNullException());
						var startupService = IPlatformApplication.Current?.Services.GetRequiredService<OnStartUpService>();
						startupService?.ScheduleGuessingPage(uri);
					}
				}));
#endif
			});

		builder.Services.AddOptions<ServerOptions>()
			.Bind(builder.Configuration.GetRequiredSection(ServerOptions.SectionName));

		builder.Services
			.AddCommon()
			.AddDAL()
			.AddBL()
			.AddViewsAndViewModels()
			.AddServices();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();

		using var scope = app.Services.CreateScope();
		var dbContext = app.Services.GetRequiredService<ApplicationDbContext>();
		dbContext.Database.Migrate();

		return app;
	}
}
