using FluentValidation;
using FluentValidation.Results;

using ShareLoc.Server.DAL.Entities;
using ShareLoc.Server.DAL.Repositories;
using ShareLoc.Server.DAL.Services;
using ShareLoc.Shared.Common.Models;

namespace ShareLoc.Server.App.Endpoints;

public sealed class Endpoints : IEndpoints
{
	public void AddEndpoints(WebApplication app)
	{
		//create place
		app.MapPost("/api/places", async (PlaceRequest request, IValidator<PlaceRequest> requestValidator, PlaceService placeService, HttpContext context, CancellationToken token) =>
		{
			ValidationResult validationResult = requestValidator.Validate(request);
			if (!validationResult.IsValid)
				return Results.BadRequest(validationResult.Errors);

			Guid placeId = await placeService.CreatePlaceAsync(request, token);

			return Results.Created($"{context.Request.Scheme}://{context.Request.Host}/api/places/{placeId}", placeId);
		});

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
		});

		//create guess
		app.MapPost("/api/places/{placeId:guid}/guesses", async (Guid placeId, GuessRequest request, IValidator<GuessRequest> requestValidator, PlaceService placeService, CancellationToken token) =>
		{
			ValidationResult validationResult = requestValidator.Validate(request);
			if (!validationResult.IsValid)
				return Results.BadRequest(validationResult.Errors);

			GuessResultResponse? response = await placeService.CreateGuessAsync(placeId, request, token);

			return response is not null ? Results.Ok(response) : Results.NotFound();
		});

		//get guesses
		app.MapGet("/api/places/{placeId:guid}/guesses", async (Guid placeId, PlaceService placeService, CancellationToken token) =>
		{
			List<Guess>? guesses = await placeService.GetGuessesByPlaceIdAsync(placeId, token);

			if (guesses is null)
				return Results.NotFound();

			return Results.Ok(guesses.Select(guess => new GuessResponse
			{
				GuesserId = guess.GuesserId,
				Distance = guess.Distance,
				Latitude = guess.Latitude,
				Longitude = guess.Longitude,
				Name = guess.Name,
				Score = guess.Score,
			}));
		});
	}
}
