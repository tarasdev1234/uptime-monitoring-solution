using System.Threading.Tasks;

namespace Messaging
{
    public interface IProducer<T>
    {
        Task SendAsync(T data);
        Task SendAsync(string key, T data);
    }
}
