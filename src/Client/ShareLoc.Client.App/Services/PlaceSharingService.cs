using ShareLoc.Client.App.Models;
using ShareLoc.Client.BL;

namespace ShareLoc.Client.App.Services;

public sealed class PlaceSharingService
{
	private readonly ServerOptions _options;

	public PlaceSharingService(ServerOptions options)
	{
		_options = options;
	}

	public Task SharePlaceUrlAsync(PlaceModel place)
	{
		return Share.RequestAsync(new ShareTextRequest
		{
			Text = $"{_options.Address}/{place.ServerId}",
			Title = place.Message
		});
	}
}
