using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging
{
    public abstract class ScopedConsumer<T> : IConsumer<T>
    {
        private readonly IServiceScopeFactory scopeFactory;

        public ScopedConsumer(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public async Task ConsumeAsync(T message, CancellationToken token)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                await InternalConsume(message, scope.ServiceProvider, token).ConfigureAwait(false);
            }
        }

        protected abstract Task InternalConsume(T message, IServiceProvider scopedServiceProvider, CancellationToken token);
    }
}
