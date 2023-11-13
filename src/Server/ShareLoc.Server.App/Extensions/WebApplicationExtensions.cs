using ShareLoc.Server.App.Endpoints;
using ShareLoc.Server.App.Pages;

namespace ShareLoc.Server.App.Extensions;

public static class WebApplicationExtensions
{
	public static WebApplication MapEndpoints<TEndpoints>(this WebApplication app) where TEndpoints : IEndpoints, new()
	{
		var endpoint = new TEndpoints();
		endpoint.AddEndpoints(app);
		return app;
	}

	public static WebApplication MapPage<TPage>(this WebApplication app) where TPage : Page, new()
	{
		var page = new TPage();
		page.Init();
		page.MapEndpoints(app);
		return app;
	}
}
