using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShareLoc.Client.DAL.Entities;

public sealed class PlaceEntity
{
	public required Guid LocalId { get; init; }

	public required Guid ServerId { get; set; }

	[Range(-90, 90)]
	public required double Latitude { get; init; }

	[Range(-180, 180)]
	public required double Longitude { get; init; }
	public required byte[] Image { get; init; }
	public required string Message { get; set; }
	public required DateTime CratedUTC { get; init; }
	public required DateTime SharedUTC { get; set; }

	public List<GuessEntity> Guesses { get; } = new();

	public bool IsShared => ServerId != Guid.Empty;
}

public sealed class PlaceEntityConfiguration : IEntityTypeConfiguration<PlaceEntity>
{
	public void Configure(EntityTypeBuilder<PlaceEntity> placeEntity)
	{
		placeEntity.HasKey(place => place.LocalId);

		placeEntity
			.Property(place => place.Message)
			.HasMaxLength(30);

		placeEntity
			.HasMany(place => place.Guesses)
			.WithOne()
			.HasForeignKey(guess => guess.LocalPlaceId);

		placeEntity.Ignore(place => place.IsShared);
	}
}
