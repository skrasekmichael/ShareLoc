using ShareLoc.Server.DAL.Entities;
using ShareLoc.Server.DAL.Repositories;
using ShareLoc.Shared.Common.Models;

namespace ShareLoc.Server.DAL.Services;

public sealed class PlaceService
{
	private const double EarthRadius = 6_371_000; //m
	private const double EarthCircumferenceHalf = 40_075_000 / 2; //m
	private const int MaxScore = 10_000;

	private readonly PlaceRepository _placeRepository;

	public PlaceService(PlaceRepository placeRepository)
	{
		_placeRepository = placeRepository;
	}

	public async Task<Guid> CreatePlaceAsync(PlaceRequest request, CancellationToken token = default)
	{
		Place newPlace = new Place()
		{
			Id = Guid.NewGuid(),
			Latitude = request.Latitude,
			Longitude = request.Longitude,
			Message = request.Message,
			Image = request.Image,
			TimeStampUTC = DateTime.UtcNow,
			Guesses = new List<Guess>()
		};

		await _placeRepository.InsertPlaceAsync(newPlace, token);

		return newPlace.Id;
	}

	public async Task<GuessResultResponse?> CreateGuessAsync(Guid placeId, GuessRequest request, CancellationToken token = default)
	{
		Place? place = await _placeRepository.GetPlaceByIdAsync(placeId, token);

		if (place is null) return null;

		double distance = CalculateDistance(request.Latitude, request.Longitude, place.Latitude, place.Longitude);
		int score = CalculateScore(distance);

		var newGuess = new Guess()
		{
			Id = Guid.NewGuid(),
			Latitude = request.Latitude,
			Longitude = request.Longitude,
			Name = request.Name,
			Score = score,
			Distance = distance,
			GuesserId = request.GuesserId
		};

		place.Guesses.Add(newGuess);

		await _placeRepository.InsertPlaceAsync(place, token);

		var response = new GuessResultResponse()
		{
			CorrectLatitude = place.Latitude,
			CorrectLongitude = place.Longitude,
			Score = score,
			Distance = distance
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
		double sinDeltaLatHalf = Math.Sin((lat2 - lat1) / 2);
		double sinDeltaLonHalf = Math.Sin((lon2 - lon1) / 2);
		double a = sinDeltaLatHalf * sinDeltaLatHalf + Math.Cos(lat1) * Math.Cos(lat2) * sinDeltaLonHalf * sinDeltaLonHalf;
		double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
		double distance = EarthRadius * c;

		return distance;
	}

	private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180f;

	private static int CalculateScore(double distance)
	{
		double normalizedDistance = distance / EarthCircumferenceHalf;
		int score = (int)(MaxScore * Math.Pow(normalizedDistance - 1, 2));

		return Math.Clamp(score, 0, MaxScore);
	}

	public async Task<List<Guess>?> GetGuessesByPlaceIdAsync(Guid placeId, CancellationToken token = default)
	{
		Place? place = await _placeRepository.GetPlaceByIdAsync(placeId, token);

		return place?.Guesses;
	}
}
