using Microsoft.Extensions.Options;

using ShareLoc.Client.App.Models;
using ShareLoc.Client.BL;

namespace ShareLoc.Client.App.Services;

public sealed class PlaceSharingService
{
	private readonly IOptions<ServerOptions> _serverOptions;

	public PlaceSharingService(IOptions<ServerOptions> serverOptions)
	{
		_serverOptions = serverOptions;
	}

	public Task SharePlaceUrlAsync(PlaceModel place)
	{
		return Share.RequestAsync(new ShareTextRequest
		{
			Text = $"{_serverOptions.Value.Address}/{place.ServerId}",
			Title = place.Message
		});
	}
}
