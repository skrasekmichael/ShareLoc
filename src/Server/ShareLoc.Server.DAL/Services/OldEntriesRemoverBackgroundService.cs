using Microsoft.Extensions.Hosting;

using ShareLoc.Server.DAL.Repositories;

namespace ShareLoc.Server.DAL.Services;

public sealed class OldEntriesRemoverBackgroundService : BackgroundService
{
	private const int RepeatAfterHours = 10;
	private const int EntryTTLDays = 30;

	private readonly PlaceRepository _placeRepository;

	public OldEntriesRemoverBackgroundService(PlaceRepository placeRepository)
	{
		_placeRepository = placeRepository;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using PeriodicTimer timer = new(TimeSpan.FromHours(RepeatAfterHours));
		try
		{
			while (await timer.WaitForNextTickAsync(stoppingToken))
			{
				await DeleteOldDBEntriesAsync(stoppingToken);
			}
		}
		catch { }
	}

	private async Task DeleteOldDBEntriesAsync(CancellationToken cancellationToken)
	{
		DateTime timeStampCutOff = DateTime.UtcNow.AddDays(-EntryTTLDays);
		string isoTimeStampCutoff = timeStampCutOff.ToString("s", System.Globalization.CultureInfo.InvariantCulture);

		var oldEntries = await _placeRepository.GetOldEntriesAsync(isoTimeStampCutoff, cancellationToken);

		await _placeRepository.DeletePlacesAsync(oldEntries, cancellationToken);
	}
}
