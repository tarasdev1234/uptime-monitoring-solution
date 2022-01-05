using System;
using System.Threading.Tasks;
using FluentAssertions;
using Cassandra.Data.Linq;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Uptime.Monitoring.Cassandra.Repositories;
using Uptime.Monitoring.Model.Models;
using System.Linq;

namespace Uptime.Monitoring.Cassandra.Tests
{
    public class RecentMonitoringEventsRepositoryTests
    {
        private const string ConnectionString = "Contact Points=localhost;Default Keyspace=uptime_test";

        private RecentMonitoringEventsRepository service;

        private IServiceProvider services;

        [SetUp]
        public void Setup()
        {
            services = new ServiceCollection()
                .AddEventsServices(ConnectionString)
                .BuildServiceProvider();

            service = services.GetRequiredService<RecentMonitoringEventsRepository>();

            var table = services.GetRequiredService<Table<RecentMonitoringEvent>>();
            table.CreateIfNotExists();
        }

        [TearDown]
        public void TearDown()
        {
            var table = services.GetRequiredService<Table<RecentMonitoringEvent>>();
            var session = table.GetSession();
            session.Execute($"TRUNCATE {table.KeyspaceName ?? session.Keyspace}.{table.Name}");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            var table = services.GetRequiredService<Table<MonitoringEvent>>();
            var session = table.GetSession();
            session.Execute($"DROP KEYSPACE {table.KeyspaceName ?? session.Keyspace}");
        }

        [Test]
        public void Should_create()
        {
            var entity = CreateEvent();

            Assert.DoesNotThrowAsync(() => service.AddAsync(entity));
        }

        [Test]
        public async Task Should_find_by_user_id()
        {
            var entity1 = CreateEvent();
            var entity2 = CreateEvent();
            entity2.UserId = 0;
            entity2.PreviousEventType = null;

            await service.AddAsync(entity1);
            await service.AddAsync(entity2);

            var result = await service.FindByUserAsync(entity1.UserId);

            result.Should().ContainSingle();
            result.First().Should().BeEquivalentTo(entity1, EquivalencyConfigurator.ForMonitoringEvent);
        }

        [Test]
        public async Task Should_find_by_user_id_and_created_time()
        {
            var entity1 = CreateEvent();
            var entity2 = CreateEvent();

            await service.AddAsync(entity1);
            await service.AddAsync(entity2);

            var result = await service.FindByUserAsync(entity1.UserId, createdAfter: entity1.Created);

            result.Should().ContainSingle();
            result.First().Should().BeEquivalentTo(entity2, EquivalencyConfigurator.ForMonitoringEvent);
        }

        [Test]
        public async Task Should_find_by_user_id_with_limit()
        {
            var limit = 1;
            var entity1 = CreateEvent();
            var entity2 = CreateEvent();

            await service.AddAsync(entity1);
            await service.AddAsync(entity2);

            var result = await service.FindByUserAsync(entity1.UserId, pagination: new Pagination { PageSize = limit });

            result.Should().HaveCount(limit);
        }

        [Test]
        public async Task Should_update()
        {
            var entity = CreateEvent();
            await service.AddAsync(entity);

            await service.UpdateAsync(
                entity.UserId,
                entity.Id,
                x => new RecentMonitoringEvent
                {
                    Repeats = entity.Repeats + 1
                });

            var result = (await service.FindByUserAsync(entity.UserId)).First();

            result.Repeats.Should().Be(entity.Repeats + 1);
        }

        private RecentMonitoringEvent CreateEvent()
        {
            return new RecentMonitoringEvent
            {
                UserId = 12,
                MonitorId = 14,
                Type = EventType.Up,
                Repeats = 1,
                SourceEventId = Guid.NewGuid(),
                LastRepeat = DateTime.UtcNow,
                Details = new MonitoringEventDetails
                {
                    { "some", "details" }
                }
            };
        }
    }
}