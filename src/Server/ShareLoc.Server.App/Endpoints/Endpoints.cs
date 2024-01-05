using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using ShareLoc.Server.DAL.Entities;
using ShareLoc.Server.DAL.Repositories;
using ShareLoc.Server.DAL.Services;
using ShareLoc.Shared.Common.Errors;
using ShareLoc.Shared.Common.Models;

namespace ShareLoc.Server.App.Endpoints;

public sealed class Endpoints : IEndpoints
{
	public void AddEndpoints(WebApplication app, string rateLimitingPolicy)
	{
		//create place
		app.MapPost("/api/places", async (PlaceRequest request, IValidator<PlaceRequest> requestValidator, PlaceService placeService, HttpContext context, CancellationToken token) =>
		{
			ValidationResult validationResult = requestValidator.Validate(request);
			if (!validationResult.IsValid)
				return Results.BadRequest(validationResult.Errors);

			Guid placeId = await placeService.CreatePlaceAsync(request, token);

			return Results.Created($"{context.Request.Scheme}://{context.Request.Host}/api/places/{placeId}", placeId);
		}).RequireRateLimiting(rateLimitingPolicy);

		//get place
		app.MapGet("/api/places/{placeId:guid}", async (Guid placeId, PlaceRepository placeRespository, CancellationToken token) =>
		{
			Place? place = await placeRespository.GetPlaceByIdAsync(placeId, token);

			if (place is null)
				return Results.NotFound();

			return Results.Ok(new PlaceResponse()
			{
				Image = place.Image,
				Message = place.Message,
				TimeStampUTC = place.TimeStampUTC
			});
		}).RequireRateLimiting(rateLimitingPolicy);

		//create guess
		app.MapPost("/api/places/{placeId:guid}/guesses", async (Guid placeId, GuessRequest request, IValidator<GuessRequest> requestValidator, PlaceService placeService, CancellationToken token) =>
		{
			ValidationResult validationResult = requestValidator.Validate(request);
			if (!validationResult.IsValid)
				return Results.BadRequest(validationResult.Errors);

			Result<GuessResultResponse> result = await placeService.CreateGuessAsync(placeId, request, token);
			if (result.IsFailed)
			{
				if (result.HasError<PlaceDoesNotExistError>())
					return Results.NotFound(result.Errors);

				if (result.HasError<GuessesCapExceededError>())
					return Results.Problem(result.Errors[0].ToString());
			}

			return Results.Ok(result.Value);

		}).RequireRateLimiting(rateLimitingPolicy);

		//get guesses
		app.MapGet("/api/places/{placeId:guid}/guesses", async (Guid placeId, PlaceService placeService, CancellationToken token) =>
		{
			List<Guess>? guesses = await placeService.GetGuessesByPlaceIdAsync(placeId, token);

			if (guesses is null)
				return Results.NotFound();

			return Results.Ok(guesses
				.OrderBy(guess => guess.Distance)
				.Select(guess => new GuessResponse
				{
					GuesserId = guess.GuesserId,
					Distance = guess.Distance,
					Latitude = guess.Latitude,
					Longitude = guess.Longitude,
					Name = guess.Name,
					Score = guess.Score,
				}));
		}).RequireRateLimiting(rateLimitingPolicy);
	}
}
