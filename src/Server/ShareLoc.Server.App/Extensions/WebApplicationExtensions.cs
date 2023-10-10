using System.Runtime.CompilerServices;

using ShareLoc.Server.App.Endpoints;

namespace ShareLoc.Server.App.Extensions;

public static class WebApplicationExtensions
{
	public static WebApplication MapEndpoints<TEndpoints>(this WebApplication app) where TEndpoints : IEndpoints, new()
	{
		var endpoint = new TEndpoints();
		endpoint.AddEndpoints(app);
		return app;
	}
}
