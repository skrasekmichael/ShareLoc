namespace ShareLoc.Server.DAL.Entities;

public sealed record Guess
{
	public required Guid Id { get; init; }
	public required Guid GuesserId { get; init; }

	public required double Latitude { get; init; }
	public required double Longitude { get; init; }
	public required string Name { get; init; }
	public required int Score { get; init; }
	public required double Distance { get; init; }
}
