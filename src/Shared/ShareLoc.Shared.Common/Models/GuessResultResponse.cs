namespace ShareLoc.Shared.Common.Models;

public sealed class GuessResultResponse
{
	public required double CorrectLatitude { get; init; }
	public required double CorrectLongitude { get; init; }
	public required int Score { get; init; }
}
