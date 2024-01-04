namespace ShareLoc.Client.App.Messages;

public sealed record PlaceSharingStateChangedMessage(Guid PlaceId, Guid ServerId) : IMessage;
