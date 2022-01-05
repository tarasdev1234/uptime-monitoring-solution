using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Uptime.Schedule
{
    public abstract class Task<T> : ITask<T> where T : notnull
    {
        public T Id { get; private set; }

        public string? Name { get; private set; }

        public ILogger? Logger { get; protected set; }

        private Action<ITask<T>>? completeAction;

        public Task(T id, string? name)
        {
            Id = id;
            Name = name;
        }

        public void Complete()
        {
            completeAction?.Invoke(this);
        }

        public abstract Task BeforeExecuteAsync();

        public abstract Task ExecuteAsync();

        public ITask<T> OnComplete(Action<ITask<T>>? action)
        {
            completeAction = action;
            return this;
        }
    }
}
