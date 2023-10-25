namespace ShareLoc.Shared.Common.Models;

public sealed class GuessResponse
{
	public required double Latitude { get; init; }
	public required double Longitude { get; init; }
	public required string Name { get; init; }
	public required int Score { get; init; }
}
