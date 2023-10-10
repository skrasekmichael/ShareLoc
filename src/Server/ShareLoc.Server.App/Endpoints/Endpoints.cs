namespace ShareLoc.Server.App.Endpoints;

public sealed class Endpoints : IEndpoints
{
	public void AddEndpoints(WebApplication app)
	{
		app.MapGet("/api/helloWorld", () => "hello world!")
			.WithName("HelloWorld")
			.WithOpenApi();
	}
}
