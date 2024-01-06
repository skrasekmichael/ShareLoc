namespace ShareLoc.Server.App.Endpoints;

public interface IEndpoints
{
	public void AddEndpoints(WebApplication app, string rateLimitingPolicy);
}
