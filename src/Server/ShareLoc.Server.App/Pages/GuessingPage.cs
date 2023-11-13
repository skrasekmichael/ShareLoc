using System.Text.Json;

using DotLiquid;

using ShareLoc.Server.DAL.Repositories;

namespace ShareLoc.Server.App.Pages;

public sealed class GuessingPage : Page
{
	private const string CookieGuessserId = "guesserId";

	public override void MapEndpoints(WebApplication app)
	{
		app.MapGet("/{placeId:guid}", GuessingPageHandler);
	}

	private async Task<IResult> GuessingPageHandler(
		Guid placeId,
		HttpContext context,
		PlaceRepository placeRepository,
		CancellationToken ct)
	{
		var place = await placeRepository.GetPlaceByIdAsync(placeId, ct);
		if (place is null)
			return Results.NotFound();

		string? guessResultJson = null;
		if (context.Request.Cookies.TryGetValue(CookieGuessserId, out var guesserIdString))
		{
			if (Guid.TryParse(guesserIdString, out var guesserId))
			{
				var guess = place.Guesses.Find(x => x.GuesserId == guesserId);

				if (guess is not null)
				{
					guessResultJson = JsonSerializer.Serialize(new
					{
						correctLongitude = place.Longitude,
						correctLatitude = place.Latitude,
						guessLongitude = guess.Longitude,
						guessLatitude = guess.Latitude
					});
				}
			}
			else
			{
				//invalid guid in cookie
				context.Response.Cookies.Delete(CookieGuessserId);
				context.Response.Cookies.Append(CookieGuessserId, Guid.NewGuid().ToString());
			}
		}
		else
		{
			context.Response.Cookies.Append(CookieGuessserId, Guid.NewGuid().ToString());
		}

		var parameters = Hash.FromAnonymousObject(new
		{
			placeId = placeId,
			text = place.Message,
			base64Image = Convert.ToBase64String(place.Image),
			myGuessInfo = guessResultJson
		});

		return Results.Text(Render(parameters), "text/html");
	}
}
