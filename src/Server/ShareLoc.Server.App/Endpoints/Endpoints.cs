using FluentValidation;
using FluentValidation.Results;

using ShareLoc.Server.DAL.Entities;
using ShareLoc.Server.DAL.Repositories;
using ShareLoc.Server.DAL.Services;
using ShareLoc.Shared.Common.Models;

namespace ShareLoc.Server.App.Endpoints;

public sealed class Endpoints : IEndpoints
{
	public Endpoints() { }

	public void AddEndpoints(WebApplication app)
	{
		app.MapPost("/api/places/{placeId:guid}/guesses", async (GuessRequest request, IValidator<GuessRequest> requestValidator, GuessService guessService, CancellationToken token) =>
		{
			ValidationResult res = requestValidator.Validate(request);
			if (!res.IsValid) return Results.BadRequest(res.Errors);

			GuessResultResponse? response = await guessService.CreateGuessAsync(request, token);

			return response is not null ? Results.Ok(response) : Results.BadRequest();
		});

		app.MapGet("/api/guesses/{guessId:guid}", async (Guid guessId, GuessService guessService, CancellationToken token) =>
		{
			GuessResponse? response = await guessService.GetGuessByIdAsync(guessId, token);

			return response is not null ? Results.Ok(response) : Results.BadRequest();
		});

		app.MapPost("/api/places", async (PlaceRequest request, IValidator<PlaceRequest> requestValidator, PlaceService placeService, CancellationToken token) =>
		{
			ValidationResult res = requestValidator.Validate(request);
			if (!res.IsValid) return Results.BadRequest(res.Errors);

			Guid placeId = await placeService.CreatePlaceAsync(request, token);

			return Results.Ok(placeId);
		});

		app.MapGet("/api/places/{placeId:guid}", async (Guid placeId, PlaceRepository placeRespository, CancellationToken token) =>
		{
			Place? place = await placeRespository.GetPlaceByIdAsync(placeId, token);

			return place is not null ? Results.Ok(place) : Results.BadRequest();
		});

		app.MapGet("/api/{guesserId:guid}/guesses", async (Guid guesserId, GuessRepository guessRepository, CancellationToken token) =>
		{
			List<Guess> guesses = await guessRepository.GetGuessesByGuesserIdAsync(guesserId, token);

			return Results.Ok(guesses);
		});
	}
}
