using EnsureThat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Uptime.Schedule
{
    public sealed class ScheduledTask<T> : IDisposable where T : notnull
    {
        public ITask<T> Task { get; private set; }

        private readonly ILogger logger;
        private readonly TimeSpan period;
        private readonly int repeat;

        private Timer? timer;
        private int iteration = 0;
        private Action<ScheduledTask<T>>? completeHandler;
        private IDisposable? loggerScope;

        public ScheduledTask(ITask<T> task, TimeSpan period, int repeat, ILogger logger)
        {
            EnsureArg.IsNotNull(task, nameof(task));
            EnsureArg.IsNotNull(logger, nameof(logger));

            Task = task;
            this.logger = logger;
            this.period = period;
            this.repeat = repeat;
        }

        public void Start()
        {
            loggerScope = logger.BeginScope(new Dictionary<string, object> { { "taskId", Task.Id.ToString() } });
            logger.LogInformation("{TaskType}:{TaskId} '{TaskName}' started.", Task.GetType().Name, Task.Id, Task.Name);
            timer = new Timer(Tick, null, TimeSpan.Zero, period);
        }

        public void Stop()
        {
            Stop(false);
        }

        public ScheduledTask<T> OnComplete(Action<ScheduledTask<T>> handler)
        {
            completeHandler = handler;
            return this;
        }

        public void Dispose()
        {
            if (timer != null)
            {
                timer.Dispose();
            }

            if (loggerScope != null)
            {
                loggerScope.Dispose();
            }
        }

        private async void Tick(object _)
        {
            if (iteration == 0)
            {
                await Task.BeforeExecuteAsync().ConfigureAwait(false);
            }

            ++iteration;

            try
            {
                var repeatStr = IsInfinite() ? "∞" : $"{repeat}";
                logger.LogInformation("{TaskType}:{TaskId} '{TaskName}' start doing work. Iteration: {Iteration}\\{Repeat}", Task.GetType().Name, Task.Id, Task.Name, iteration, repeatStr);
                await Task.ExecuteAsync().ConfigureAwait(false);
                logger.LogInformation("{TaskType}:{TaskId} '{TaskName}' finished its work. Iteration: {Iteration}\\{Repeat}", Task.GetType().Name, Task.Id, Task.Name, iteration, repeatStr);

                if (!IsInfinite() && iteration >= repeat)
                {
                    Stop(true);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Scheduled task '{TaskName}' throws an exception", Task.Name);
            }
        }

        private void Stop(bool wasCompleted)
        {
            var action = wasCompleted ? "completed" : "stopped";
            logger.LogInformation("{TaskType}:{TaskId} '{TaskName}' {Action}.", GetType().Name, Task.Id, Task.Name, action);

            Dispose();
            completeHandler?.Invoke(this);
        }

        private bool IsInfinite()
        {
            return repeat <= 0;
        }
    }
}
