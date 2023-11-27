using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using ShareLoc.Client.App.Extensions;
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
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
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
