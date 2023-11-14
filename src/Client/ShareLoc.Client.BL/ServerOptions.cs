namespace ShareLoc.Client.BL;

public sealed class ServerOptions
{
	public const string SectionName = "Server";

	public required string Address { get; init; }
}
