namespace ShareLoc.Shared.Common.Models;

public sealed class PlaceResponse
{
	public required string Message { get; init; }
	public required byte[] Image { get; init; }
	public required DateTime TimeStampUTC { get; init; }
}
