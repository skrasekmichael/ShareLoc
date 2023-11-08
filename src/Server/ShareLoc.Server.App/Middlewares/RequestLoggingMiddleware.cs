namespace ShareLoc.Server.App.Middlewares;

public sealed class RequestLoggingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<RequestLoggingMiddleware> _logger;

	public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task Invoke(HttpContext context)
	{
		var cookies = "";
		if (context.Request.Cookies.Count > 0)
		{
			foreach (var cookie in context.Request.Cookies)
			{
				cookies += $"\n\tCookie: {cookie.Key} - {cookie.Value}";
			}
		}

		_logger.LogInformation("Request: {method} {path} from {remoteHost} {cookies}", context.Request.Method, context.Request.Path, context.Connection.RemoteIpAddress, cookies);

		await _next(context);
	}
}
