using System.ComponentModel.DataAnnotations;

using LiteDB;

namespace ShareLoc.Client.DAL.Entities;

public sealed class PlaceEntity
{
	[Key]
	[BsonId]
	public required Guid LocalId { get; init; }

	public required Guid ServerId { get; set; }

	[Range(-90, 90)]
	public required double Latitude { get; init; }

	[Range(-180, 180)]
	public required double Longitude { get; init; }
	public required byte[] Image { get; init; }

	[Range(0, 30)]
	public required string Message { get; set; }
	public required DateTime CreatedUTC { get; init; }
	public required DateTime SharedUTC { get; set; }

	public bool IsShared => ServerId != Guid.Empty;
}
