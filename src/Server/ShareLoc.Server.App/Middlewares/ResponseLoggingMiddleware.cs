namespace ShareLoc.Server.App.Middlewares;

public sealed class ResponseLoggingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<RequestLoggingMiddleware> _logger;

	public ResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task Invoke(HttpContext context)
	{
		await _next(context);

		string headers = "";
		foreach (var header in context.Response.Headers)
		{
			headers += $"\n\tHeader: {header.Key} - {string.Join(", ", header.Value.ToList())}";
		}

		_logger.LogInformation("Response: {statusCode}{headers}", context.Response.StatusCode, headers);
	}
}
