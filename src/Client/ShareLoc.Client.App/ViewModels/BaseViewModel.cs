using AsyncAwaitBestPractices;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ShareLoc.Client.App.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
	public bool IsLoaded { get; private set; } = false;

	[ObservableProperty]
	private bool _isLoading = false;

	public void LoadAsynchronously(bool forceLoad = false, CancellationToken ct = default)
	{
		if (!IsLoading && (!IsLoaded || forceLoad))
		{
			IsLoading = true;
			Task.Run(async () =>
			{
				await LoadAsync(ct);
				IsLoaded = true;
				IsLoading = false;
			}, ct).SafeFireAndForget();
		}
	}

	protected virtual Task LoadAsync(CancellationToken ct) => Task.CompletedTask;

	protected Task Dispatch(Action action) => MainThread.InvokeOnMainThreadAsync(action);

	protected Task Dispatch(Func<Task> asyncAction) => MainThread.InvokeOnMainThreadAsync(asyncAction);

	protected Task<TResult> Dispatch<TResult>(Func<Task<TResult>> asyncAction) => MainThread.InvokeOnMainThreadAsync(asyncAction);
}
