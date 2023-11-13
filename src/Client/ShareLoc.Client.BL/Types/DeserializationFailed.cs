namespace ShareLoc.Client.BL.Types;

public readonly struct DeserializationFailed : IUnexpectedError
{
	public string Name { get; }

	public DeserializationFailed()
	{
		Name = "Response Deserialization Failed";
	}
}
