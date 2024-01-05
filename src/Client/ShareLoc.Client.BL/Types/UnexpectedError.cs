namespace ShareLoc.Client.BL.Types;

public readonly struct UnexpectedError : IUnexpectedError
{
	public string Name { get; }

	public UnexpectedError(string name)
	{
		Name = name;
	}
}
