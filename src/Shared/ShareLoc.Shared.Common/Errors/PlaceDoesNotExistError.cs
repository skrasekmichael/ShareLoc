using FluentResults;

namespace ShareLoc.Shared.Common.Errors;

public sealed class PlaceDoesNotExistError : Error
{
	public PlaceDoesNotExistError(Guid placeId) : base($"Place with ID {placeId} does not exist.") { }
}
