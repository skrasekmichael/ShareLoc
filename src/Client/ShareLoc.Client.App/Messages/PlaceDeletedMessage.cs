namespace ShareLoc.Client.App.Messages;

public sealed record PlaceDeletedMessage(Guid PlaceId) : IMessage;
