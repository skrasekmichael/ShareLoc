using ShareLoc.Client.App.Models;

namespace ShareLoc.Client.App.Messages;

public sealed record PlaceStoredMessage(PlaceModel Model) : IMessage;
