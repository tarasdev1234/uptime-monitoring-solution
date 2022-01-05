using System.Threading;
using System.Threading.Tasks;

namespace Messaging
{
    public interface IConsumer<T>
    {
        Task ConsumeAsync(T message, CancellationToken token);
    }
}
