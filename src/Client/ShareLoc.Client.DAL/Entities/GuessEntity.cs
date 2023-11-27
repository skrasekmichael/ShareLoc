using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ShareLoc.Client.DAL.Entities;

public sealed class GuessEntity
{
	public required Guid LocalId { get; init; }

	public required Guid ServerId { get; init; }
	public required Guid LocalPlaceId { get; init; }
	public required Guid GuesserId { get; init; }

	[Range(-90, 90)]
	public required double Latitude { get; init; }

	[Range(-180, 180)]
	public required double Longitude { get; init; }
	public required string Name { get; init; }
	public required int Score { get; init; }
	public required double Distance { get; init; }
}

public sealed class GuessEntityConfiguration : IEntityTypeConfiguration<GuessEntity>
{
	public void Configure(EntityTypeBuilder<GuessEntity> guessEntity)
	{
		guessEntity.HasKey(place => place.LocalId);

		guessEntity.HasOne<PlaceEntity>();

		guessEntity
			.Property(guess => guess.Name)
			.HasMaxLength(30);
	}
}
