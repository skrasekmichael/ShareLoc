using ShareLoc.Server.DAL.Entities;
using ShareLoc.Server.DAL.Repositories;
using ShareLoc.Shared.Common.Models;

namespace ShareLoc.Server.DAL.Services;

public sealed class GuessService
{
	private const double EarthRadius = 6371;
	private const int EarthCircumferenceHalf = 40075 / 2;
	private const int MaxScore = 10000;
	private const double NormalizationConstant = MaxScore / EarthCircumferenceHalf;

	private readonly PlaceRepository _placeRepository;
	private readonly GuessRepository _guessRepository;

	public GuessService(PlaceRepository placeRepository, GuessRepository guessRepository)
	{
		_placeRepository = placeRepository;
		_guessRepository = guessRepository;
	}

	public async Task<GuessResponse?> GetGuessByIdAsync(Guid id)
	{
		Guess? guess = await _guessRepository.GetGuessByIdAsync(id);

		if (guess == null) return null;

		GuessResponse response = new GuessResponse()
		{
			Latitude = guess.Latitude,
			Longitude = guess.Longitude,
			Name = guess.Name,
			Score = guess.Score
		};

		return response;
	}

	public async Task<GuessResponse?> CreateGuessAsync(GuessRequest request)
	{
		Place? place = await _placeRepository.GetPlaceByIdAsync(request.PlaceId);

		if (place is null) return null;

		int score = EarthCircumferenceHalf - (int)CalculateDistance(request.Latitude, request.Longitude, place.Latitude, place.Longitude);
		int normalizedScore = (int)(score * NormalizationConstant);

		Guess newGuess = new Guess()
		{
			Id = Guid.NewGuid(),
			Latitude = request.Latitude,
			Longitude = request.Longitude,
			Name = request.Name,
			Score = normalizedScore,
			PlaceId = request.PlaceId
		};

		await _guessRepository.InsertGuessAsync(newGuess);

		GuessResponse response = new GuessResponse()
		{
			Latitude = newGuess.Latitude,
			Longitude = newGuess.Longitude,
			Name = newGuess.Name,
			Score = newGuess.Score
		};

		return response;
	}

	private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
	{
		// Convert latitude and longitude from degrees to radians
		lat1 = DegreesToRadians(lat1);
		lon1 = DegreesToRadians(lon1);
		lat2 = DegreesToRadians(lat2);
		lon2 = DegreesToRadians(lon2);

		// Haversine formula
		double dLat = lat2 - lat1;
		double dLon = lon2 - lon1;
		double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
				   Math.Cos(lat1) * Math.Cos(lat2) *
				   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
		double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
		double distance = EarthRadius * c;

		return distance;
	}

	private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180f;
}
