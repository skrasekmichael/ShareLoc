using System.Timers;

namespace ShareLoc.Client.BL.Services;

public class DatabasePollingService
{
	private readonly System.Timers.Timer _timer;

	public DatabasePollingService()
	{
		_timer = new System.Timers.Timer(1000);
		_timer.AutoReset = true;
		_timer.Elapsed += TimeElapsedHandler;
	}

	public void Start()
	{
		PollForData();
		StartTimer();
	}
	public void Stop() => StopTimer();

	private void StartTimer() => _timer.Start();
	private void StopTimer() => _timer?.Stop();

	private void TimeElapsedHandler(object? sender, ElapsedEventArgs e) => PollForData();
	private void PollForData()
	{
		// Poll for data
	}
}
