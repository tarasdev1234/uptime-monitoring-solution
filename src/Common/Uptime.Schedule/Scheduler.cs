using EnsureThat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Uptime.Schedule
{
    public sealed class Scheduler<T> : IDisposable where T : notnull
    {
        private static readonly TimeSpan DefaultMinimalPeriod = TimeSpan.FromSeconds(60);
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger logger;
        private readonly TimeSpan minimalPeriod;
        private readonly ConcurrentDictionary<T, ScheduledTask<T>> tasks = new ConcurrentDictionary<T, ScheduledTask<T>>();
        private readonly ConcurrentDictionary<ITask<T>, Task> onetimeTasks = new ConcurrentDictionary<ITask<T>, Task>();

        public Scheduler(ILoggerFactory loggerFactory, TimeSpan? minimalPeriod = null)
        {
            this.minimalPeriod = minimalPeriod ?? DefaultMinimalPeriod;
            this.loggerFactory = loggerFactory;
            logger = loggerFactory.CreateLogger<Scheduler<T>>();
        }

        public bool IsScheduled(T taskId) => tasks.TryGetValue(taskId, out _);

        public ICollection<T> GetTasksIds() => tasks.Keys;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
        public void Schedule(ITask<T> task, TimeSpan period, int repeat = 0)
        {
            EnsureArg.IsNotNull(task, nameof(task));

            var scheduledTask = new ScheduledTask<T>(
                task,
                period > minimalPeriod ? period : minimalPeriod,
                repeat,
                task.Logger ?? loggerFactory.CreateLogger($"Task-{task.Id}"))
                .OnComplete(HandleCompletedTask);

            if (tasks.TryAdd(task.Id, scheduledTask))
            {
                scheduledTask.Start();
            }
            else
            {
                scheduledTask.Dispose();
                throw new Exception($"Task with id {task.Id} has been started already");
            }
        }

        public void StartOnce(ITask<T> task)
        {
            EnsureArg.IsNotNull(task, nameof(task));

            try
            {
                var asyncTask = new Task(async () =>
                {
                    logger.LogInformation("{TaskType}:{TaskId} '{TaskName}' start doing work", task.GetType().Name, task.Id, task.Name);
                    await task.ExecuteAsync().ConfigureAwait(false);
                    logger.LogInformation("{TaskType}:{TaskId} '{TaskName}' finished its work", task.GetType().Name, task.Id, task.Name);
                    task.Complete();
                    onetimeTasks.TryRemove(task, out _);
                });

                if (onetimeTasks.TryAdd(task, asyncTask))
                {
                    asyncTask.Start(TaskScheduler.Default);
                }
                else
                {
                    throw new Exception($"Can't start onetime task '{task.Name}'.");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "One time task '{TaskName}' throws an exception", task.Name);
            }
        }

        /// <summary>
        /// Stop task
        /// </summary>
        /// <param name="taskId">Task id</param>
        /// <returns>True if tasks stopped succesfully.
        /// False - if task not found</returns>
        public bool TryStop(T taskId)
        {
            if (tasks.TryGetValue(taskId, out var task))
            {
                task.Stop();
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            foreach (var task in tasks)
            {
                task.Value?.Dispose();
            }

            tasks.Clear();
        }

        private void HandleCompletedTask(ScheduledTask<T> task)
        {
            tasks.TryRemove(task.Task.Id, out var _);
            task.Task.Complete();
        }
    }
}
