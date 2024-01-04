global using ValidationErrors = System.Collections.Generic.List<FluentValidation.Results.ValidationFailure>;

using System.Net;
using System.Net.Http.Json;

using OneOf;
using OneOf.Types;

using ShareLoc.Client.BL.Extensions;
using ShareLoc.Client.BL.Types;
using ShareLoc.Shared.Common.Models;

namespace ShareLoc.Client.BL.Services;

public sealed class ApiClient
{
	private readonly HttpClient _httpClient;

	public ApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<OneOf<Guid, ValidationErrors, IUnexpectedError>> CreatePlaceAsync(PlaceRequest place, CancellationToken ct)
	{
		try
		{
			var response = await _httpClient.PostAsJsonAsync("/api/places", place, ct);

			return response.StatusCode switch
			{
				HttpStatusCode.Created => await response.GetFromJsonAsync<Guid>(ct),
				HttpStatusCode.BadRequest => await response.GetValidationErrorsAsync(ct),
				_ => await response.GetHttpErrorAsync(ct),
			};
		}
		catch (Exception ex)
		{
			return new UnexpectedError(ex.Message);
		}
	}

	public async Task<OneOf<GuessResultResponse, ValidationErrors, NotFound, IUnexpectedError>> CreateGuessAsync(Guid placeId, GuessRequest guess, CancellationToken ct)
	{
		try
		{
			var response = await _httpClient.PostAsJsonAsync($"/api/places/{placeId}/guesses", guess, ct);

			return response.StatusCode switch
			{
				HttpStatusCode.OK => await response.GetFromJsonAsync<GuessResultResponse>(ct) switch
				{
					null => new DeserializationFailed(),
					var guessResult => guessResult
				},
				HttpStatusCode.BadRequest => await response.GetValidationErrorsAsync(ct),
				HttpStatusCode.NotFound => new NotFound(),
				_ => await response.GetHttpErrorAsync(ct),
			};
		}
		catch (Exception ex)
		{
			return new UnexpectedError(ex.Message);
		}
	}

	public async Task<OneOf<PlaceResponse, NotFound, IUnexpectedError>> GetPlaceAsync(Guid placeId, CancellationToken ct)
	{
		try
		{
			var response = await _httpClient.GetAsync($"/api/places/{placeId}", ct);

			return response.StatusCode switch
			{
				HttpStatusCode.OK => await response.GetFromJsonAsync<PlaceResponse>(ct) switch
				{
					null => new DeserializationFailed(),
					var place => place
				},
				HttpStatusCode.NotFound => new NotFound(),
				_ => await response.GetHttpErrorAsync(ct)
			};
		}
		catch (Exception ex)
		{
			return new UnexpectedError(ex.Message);
		}
	}

	public async Task<OneOf<List<GuessResponse>, NotFound, IUnexpectedError>> GetGuessesAsync(Guid placeId, CancellationToken ct)
	{
		try
		{
			var response = await _httpClient.GetAsync($"/api/places/{placeId}/guesses", ct);

			return response.StatusCode switch
			{
				HttpStatusCode.OK => await response.GetFromJsonAsync<List<GuessResponse>>(ct) switch
				{
					null => new DeserializationFailed(),
					var guesses => guesses
				},
				HttpStatusCode.NotFound => new NotFound(),
				_ => await response.GetHttpErrorAsync(ct),
			};
		}
		catch (Exception ex)
		{
			return new UnexpectedError(ex.Message);
		}
	}
}
