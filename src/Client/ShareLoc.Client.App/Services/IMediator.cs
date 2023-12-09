using CommunityToolkit.Mvvm.Messaging;

using ShareLoc.Client.App.Messages;

namespace ShareLoc.Client.App.Services;

public interface IMediator
{
	IMessenger Messenger { get; }

	void Publish<TMessage>(TMessage message) where TMessage : class, IMessage;
	void Subscribe<TSubscriber, TMessage>(TSubscriber subscriber, MessageHandler<TSubscriber, TMessage> handler)
		where TSubscriber : class
		where TMessage : class, IMessage;
	void Unsubscribe<TSubscriber, TMessage>(TSubscriber subscriber)
		where TSubscriber : class
		where TMessage : class, IMessage;
	void UnsubscribeAll<TSubscriber>(TSubscriber subscriber) where TSubscriber : class;
}
