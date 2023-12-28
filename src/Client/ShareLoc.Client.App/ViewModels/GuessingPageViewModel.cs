using ShareLoc.Client.App.Services;

namespace ShareLoc.Client.App.ViewModels;

public sealed partial class GuessingPageViewModel : BaseViewModel
{
	public WebView WebView { get; } = new();

	public ProfileWrapper Profile { get; }

	public Uri? Uri { get; set; }

	public GuessingPageViewModel(ProfileWrapper profileWrapper)
	{
		Profile = profileWrapper;

		WebView.Navigating += async (_, _) =>
		{
			await WebView.EvaluateJavaScriptAsync($"""localStorage.setItem("appAccess", "TRUE");""");
			await WebView.EvaluateJavaScriptAsync($"""localStorage.setItem("guesserName", "{Profile.Name}");""");
			await WebView.EvaluateJavaScriptAsync($"""document.cookie = "guesserId='{Profile.UserId}'; path=/";""");
		};
	}

	protected override Task LoadAsync(CancellationToken ct)
	{
		WebView.Source = Uri;
		return Task.CompletedTask;
	}
}
