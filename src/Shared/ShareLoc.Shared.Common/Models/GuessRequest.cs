using FluentValidation;

namespace ShareLoc.Shared.Common.Models;

public sealed class GuessRequest
{
	public required Guid PlaceId { get; init; }
	public required Guid GuesserId { get; init; }
	public required double Latitude { get; init; }
	public required double Longitude { get; init; }
	public required string Name { get; init; }
}

public sealed class GuessRequestValidator : AbstractValidator<GuessRequest>
{
	public GuessRequestValidator()
	{
		RuleFor(x => x.Latitude)
			.InclusiveBetween(-90, 90)
			.WithMessage("Latitude has to be number between -90 and 90.");

		RuleFor(x => x.Longitude)
			.InclusiveBetween(-180, 180)
			.WithMessage("Longitude has to be number between -180 and 180.");

		RuleFor(x => x.Name)
			.MaximumLength(30)
			.WithMessage("Name has maximum length of 30 characters.");
	}
}
