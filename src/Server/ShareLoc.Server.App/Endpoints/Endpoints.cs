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
		app.MapPost("/api/places", async (PlaceRequest request, IValidator<PlaceRequest> requestValidator, PlaceService placeService, CancellationToken token) =>
		{
			ValidationResult res = requestValidator.Validate(request);
			if (!res.IsValid) return Results.BadRequest(res.Errors);

			Guid placeId = await placeService.CreatePlaceAsync(request, token);

			return Results.Ok(placeId);
		});

		//get place
		app.MapGet("/api/places/{placeId:guid}", async (Guid placeId, PlaceRepository placeRespository, CancellationToken token) =>
		{
			Place? place = await placeRespository.GetPlaceByIdAsync(placeId, token);

			return place is not null ? Results.Ok(place) : Results.BadRequest();
		});

		//create guess
		app.MapPost("/api/places/{placeId:guid}/guesses", async (Guid placeId, GuessRequest request, IValidator<GuessRequest> requestValidator, PlaceService placeService, CancellationToken token) =>
		{
			ValidationResult res = requestValidator.Validate(request);
			if (!res.IsValid) return Results.BadRequest(res.Errors);

			GuessResultResponse? response = await placeService.CreateGuessAsync(placeId, request, token);

			return response is not null ? Results.Ok(response) : Results.BadRequest();
		});

		//get guesses
		app.MapGet("/api/places/{placeId:guid}/guesses", async (Guid placeId, PlaceService placeService, CancellationToken token) =>
		{
			List<Guess>? guesses = await placeService.GetGuessesByPlaceIdAsync(placeId, token);

			return guesses is not null ? Results.Ok(guesses) : Results.NotFound();
		});
	}
}
