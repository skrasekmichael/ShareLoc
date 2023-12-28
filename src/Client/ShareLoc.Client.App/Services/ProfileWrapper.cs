namespace ShareLoc.Client.App.Services;

public sealed class ProfileWrapper
{
	private const string NAME = "user-name";
	private const string ID = "user-id";

	private string? _name;
	public string Name
	{
		get
		{
			_name ??= Preferences.Get(NAME, "");
			return _name;
		}
		set => Preferences.Set(NAME, value);
	}

	private Guid? _userId;
	public Guid UserId
	{
		get
		{
			if (!Preferences.ContainsKey(ID))
			{
				_userId = Guid.NewGuid();
				Preferences.Set(ID, _userId.ToString());
			}

			_userId ??= Guid.Parse(Preferences.Get(ID, ""));
			return _userId.Value;
		}
	}
}
