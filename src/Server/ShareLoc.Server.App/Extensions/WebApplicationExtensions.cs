using ShareLoc.Server.App.Endpoints;
using ShareLoc.Server.App.Pages;

namespace ShareLoc.Server.App.Extensions;

public static class WebApplicationExtensions
{
	public static WebApplication MapEndpoints<TEndpoints>(this WebApplication app) where TEndpoints : IEndpoints, new()
	{
		string rateLimitingPolicy = app.Configuration["RateLimiting:Policy"] ?? throw new Exception("Missing rate limiting configuration.");

		var endpoint = new TEndpoints();
		endpoint.AddEndpoints(app, rateLimitingPolicy);
		return app;
	}

	public static WebApplication MapPage<TPage>(this WebApplication app) where TPage : Page, new()
	{
		string rateLimitingPolicy = app.Configuration["RateLimiting:Policy"] ?? throw new Exception("Missing rate limiting configuration.");

		var page = new TPage();
		page.Init();
		page.MapEndpoints(app, rateLimitingPolicy);
		return app;
	}
}
