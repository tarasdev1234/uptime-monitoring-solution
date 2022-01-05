using System;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging
{
    public interface IQueue
    {
        void StartListening(CancellationToken token);

        Task WaitFinishAsync();
    }

    public interface IQueue<T> : IQueue
    {
        void Subscribe(IConsumer<T> consumer);

        IProducer<T> GetProducer();
    }
}
