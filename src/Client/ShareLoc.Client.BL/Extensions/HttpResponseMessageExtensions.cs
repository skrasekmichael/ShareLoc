using System.Net.Http.Json;

using ShareLoc.Client.BL.Types;

namespace ShareLoc.Client.BL.Extensions;

public static class HttpResponseMessageExtensions
{
	private static readonly ValidationErrors EmptyErrors = new();

	public static async Task<UnexpectedResponse> GetHttpErrorAsync(this HttpResponseMessage response, CancellationToken ct)
	{
		return new UnexpectedResponse(
			response.StatusCode,
			await response.Content.ReadAsStringAsync(ct)
		);
	}

	public static async Task<T?> GetFromJsonAsync<T>(this HttpResponseMessage response, CancellationToken ct)
	{
		return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
	}

	public static async Task<ValidationErrors> GetValidationErrorsAsync(this HttpResponseMessage response, CancellationToken ct)
	{
		return await response.Content.ReadFromJsonAsync<ValidationErrors>(cancellationToken: ct) ?? EmptyErrors;
	}
}
