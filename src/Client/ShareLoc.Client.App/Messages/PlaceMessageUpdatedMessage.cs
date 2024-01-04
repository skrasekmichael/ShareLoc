namespace ShareLoc.Client.App.Messages;

public sealed record PlaceMessageUpdatedMessage(Guid PlaceId, string Message) : IMessage;
