using CommunityToolkit.Mvvm.Messaging;

using ShareLoc.Client.App.Messages;

namespace ShareLoc.Client.App.Services;

public sealed class Mediator : IMediator
{
	public IMessenger Messenger { get; }

	public Mediator(IMessenger messenger)
	{
		Messenger = messenger;
	}

	public void Publish<TMessage>(TMessage message) where TMessage : class, IMessage
	{
		Messenger.Send(message);
	}

	public void Subscribe<TSubscriber, TMessage>(TSubscriber subscriber, MessageHandler<TSubscriber, TMessage> handler)
		where TSubscriber : class
		where TMessage : class, IMessage
	{
		Messenger.Register(subscriber, handler);
	}

	public void Unsubscribe<TSubscriber, TMessage>(TSubscriber subscriber)
		where TSubscriber : class
		where TMessage : class, IMessage
	{
		Messenger.Unregister<TMessage>(subscriber);
	}

	public void UnsubscribeAll<TSubscriber>(TSubscriber subscriber) where TSubscriber : class
	{
		Messenger.UnregisterAll(subscriber);
	}
}
