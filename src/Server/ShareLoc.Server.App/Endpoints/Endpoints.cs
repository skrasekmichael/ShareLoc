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
		app.MapPost("/api/places/{placeId:guid}/guesses", async (GuessRequest request, GuessService guessService) =>
		{
			GuessResultResponse? response = await guessService.CreateGuessAsync(request);

			return response is not null ? Results.Ok(response) : Results.BadRequest();
		});

		app.MapGet("/api/guesses/{id}", async (Guid id, GuessService guessService) =>
		{
			GuessResponse? response = await guessService.GetGuessByIdAsync(id);

			return response is not null ? Results.Ok(response) : Results.BadRequest();
		});

		app.MapPost("/api/places", async (PlaceRequest request, PlaceService placeService) =>
		{
			Guid id = await placeService.CreatePlaceAsync(request);

			Results.Ok(id);
		});

		app.MapGet("/api/places/{placeId:guid}", async (Guid placeId, PlaceRepository placeRespository) =>
		{
			Place? place = await placeRespository.GetPlaceByIdAsync(placeId);

			return place is not null ? Results.Ok(place) : Results.BadRequest();
		});

		app.MapGet("/api/{guesserId:guid}/guesses", async (Guid guesserId, GuessRepository guessRepository) =>
		{
			List<Guess> guesses = await guessRepository.GetGuessesByGuesserIdAsync(guesserId);

			return Results.Ok(guesses);
		});
	}
}
