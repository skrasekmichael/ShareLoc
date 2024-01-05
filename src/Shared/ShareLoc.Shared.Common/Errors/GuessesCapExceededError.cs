using FluentResults;

namespace ShareLoc.Shared.Common.Errors;

public sealed class GuessesCapExceededError : Error
{
	public GuessesCapExceededError(Guid placeId, int guessesCap) : base($"Number of guesses ({guessesCap}) for place with id {placeId} was exceeded.") { }
}
