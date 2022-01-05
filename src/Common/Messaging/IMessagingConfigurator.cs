using System;

namespace Messaging
{
    public interface IMessagingConfigurator
    {
        IQueueConfigurator<T> Queue<T>(string queueName);
    }

    public interface IQueueConfigurator<T> : IMessagingConfigurator
    {
        IQueueConfigurator<T> WithListener(Func<IServiceProvider, IConsumer<T>> consumerFactory);
        IQueueConfigurator<T> WithListener<TConsumer>() where TConsumer : IConsumer<T>;
    }
}
