using FluentValidation;

namespace ShareLoc.Shared.Common.Models;

public sealed class PlaceRequest
{
	public required double Latitude { get; init; }
	public required double Longitude { get; init; }
	public required string Message { get; init; }
	public required byte[] Image { get; init; }
}

public sealed class PlaceRequestValidator : AbstractValidator<PlaceRequest>
{
	private const int MaxImageSizeInBytes = 399_360;

	public PlaceRequestValidator()
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

		RuleFor(x => x.Image.Length)
			.LessThanOrEqualTo(MaxImageSizeInBytes)
			.WithMessage("Image has a maximum size of 390kB.");
	}
}
