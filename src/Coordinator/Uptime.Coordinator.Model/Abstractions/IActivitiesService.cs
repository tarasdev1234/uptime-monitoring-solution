using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Coordinator.Model.Models;

namespace Uptime.Coordinator.Model.Abstractions
{
    public interface IActivitiesService
    {
        Task<Activity> GetActivityAsync(long monitorId, CancellationToken token);

        Task<IReadOnlyList<Activity>> GetActivitiesByExecutorAsync(Guid executorId, CancellationToken token);

        Task<IReadOnlyList<Activity>> GetActivitiesByExecutorsAsync(IEnumerable<Guid> executorIds, CancellationToken token);

        Task<IReadOnlyList<Guid>> GetActiveExecutorsAsync(CancellationToken token);

        Task AddActivityAsync(Activity activity, CancellationToken token);

        Task UpdateActivityAsync(Activity activity, CancellationToken token);

        Task DeleteActivityAsync(long monitorId, CancellationToken token);

        Task DeleteActivitiesAsync(IEnumerable<long> monitorIds, CancellationToken token);

        Task CorrectLiveExecutorsAsync(IEnumerable<Guid> liveExecutorIds, CancellationToken token);
    }
}
