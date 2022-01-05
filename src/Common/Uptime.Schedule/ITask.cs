using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Uptime.Schedule
{
    public interface ITask<T> where T : notnull
    {
        T Id { get; }

        string? Name { get; }

        ILogger? Logger { get; }

        void Complete();

        Task BeforeExecuteAsync();

        Task ExecuteAsync();

        ITask<T> OnComplete(Action<ITask<T>>? action);
    }
}
