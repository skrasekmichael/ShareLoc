using FluentValidation;

namespace ShareLoc.Shared.Common.Models;

public sealed class CreatePlaceRequest
{
	public required double Latitude { get; init; }
	public required double Longitude { get; init; }
	public required string Message { get; init; }
	public required byte[] Image { get; init; }
}

public sealed class CreatePlaceRequestValidator : AbstractValidator<CreatePlaceRequest>
{
	public CreatePlaceRequestValidator()
	{
		RuleFor(x => x.Latitude)
			.InclusiveBetween(-90, 90)
			.WithMessage("Latitude has to be number between -90 and 90.");

		RuleFor(x => x.Longitude)
			.InclusiveBetween(-180, 180)
			.WithMessage("Longitude has to be number between -180 and 180.");

		RuleFor(x => x.Message)
			.MaximumLength(30)
			.WithMessage("Message has maximum length of 30 characters.");
	}
}
