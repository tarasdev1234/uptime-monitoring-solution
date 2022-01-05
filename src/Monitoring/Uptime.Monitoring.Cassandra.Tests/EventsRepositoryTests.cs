using Cassandra;
using Cassandra.Data.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using Uptime.Monitoring.Cassandra.Repositories;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Cassandra.Tests
{
    [TestFixture]
    public sealed class EventsRepositoryTests
    {
        private const string ConnectionString = "Contact Points=localhost;Default Keyspace=uptime_test";

        private EventsRepository service;

        private IServiceProvider services;

        [SetUp]
        public void Setup()
        {
            services = new ServiceCollection()
                .AddEventsServices(ConnectionString)
                .BuildServiceProvider();

            service = services.GetRequiredService<EventsRepository>();

            var table = services.GetRequiredService<Table<MonitoringEvent>>();
            table.CreateIfNotExists();
        }

        [TearDown]
        public void TearDown()
        {
            var table = services.GetRequiredService<Table<MonitoringEvent>>();
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
        public async Task Should_update()
        {
            var newFieldValue = 5L;
            var entity = CreateEvent();

            await service.AddAsync(entity);

            Assert.DoesNotThrowAsync(() => service.UpdateAsync(entity.UserId, entity.MonitorId, entity.Id, x => new MonitoringEvent { Repeats = newFieldValue }));

            var result = await service.FindAsync(entity.UserId, entity.MonitorId);

            result.Should().ContainSingle();
            result.First().Should().BeEquivalentTo(entity, cfg => EquivalencyConfigurator.ForMonitoringEvent(cfg).Excluding(x => x.Repeats));
            result.First().Repeats.Should().Be(newFieldValue);
        }

        [Test]
        public async Task Should_find_by_monitor_id()
        {
            var entity1 = CreateEvent();
            var entity2 = CreateEvent();
            entity2.MonitorId = 0;
            entity2.PreviousEventType = null;

            await service.AddAsync(entity1);
            await service.AddAsync(entity2);

            var result = await service.FindAsync(entity1.UserId, entity1.MonitorId);

            result.Should().ContainSingle();
            result.First().Should().BeEquivalentTo(entity1, EquivalencyConfigurator.ForMonitoringEvent);
        }

        [Test]
        public async Task Should_find_by_monitor_ids()
        {
            var entity1 = CreateEvent();
            var entity2 = CreateEvent();
            var entity3 = CreateEvent();
            entity2.MonitorId = 0;
            entity3.MonitorId = 3;

            await service.AddAsync(entity1);
            await service.AddAsync(entity2);
            await service.AddAsync(entity3);

            var result = await service.FindAsync(entity1.UserId, new[] { entity1.MonitorId, entity2.MonitorId }, null);

            result.Should().HaveCount(2);
        }

        [Test]
        public async Task Should_find_by_monitor_ids_and_created_time()
        {
            var entity1 = CreateEvent();
            var entity2 = CreateEvent();
            var entity3 = CreateEvent();
            entity2.MonitorId = 0;
            entity3.MonitorId = 3;

            await service.AddAsync(entity1);
            await Task.Delay(500);
            await service.AddAsync(entity2);
            await service.AddAsync(entity3);

            var result = await service.FindAsync(entity1.UserId, new[] { entity1.MonitorId, entity2.MonitorId }, ((TimeUuid)entity1.Id).GetDate());

            result.Should().ContainSingle();
            result.First().Should().BeEquivalentTo(entity2, EquivalencyConfigurator.ForMonitoringEvent);
        }

        [Test]
        public async Task Should_find_by_monitor_id_with_limit()
        {
            var limit = 1;
            var entity1 = CreateEvent();
            var entity2 = CreateEvent();

            await service.AddAsync(entity1);
            await service.AddAsync(entity2);

            var result = await service.FindAsync(entity1.UserId, entity1.MonitorId, pagination: new Pagination { PageSize = limit });

            result.Should().HaveCount(limit);
        }

        [Test]
        public async Task Should_delete()
        {
            var entity1 = CreateEvent();
            var entity2 = CreateEvent();

            await service.AddAsync(entity1);
            await service.AddAsync(entity2);

            await service.DeleteAsync(entity1.UserId, entity1.MonitorId);

            var result = await service.FindAsync(entity1.UserId, entity1.MonitorId);
            result.Should().BeEmpty();
        }

        private MonitoringEvent CreateEvent()
        {
            return new MonitoringEvent
            {
                Id = TimeUuid.NewId(),
                MonitorId = 14,
                Type = EventType.Up,
                LastRepeat = DateTime.UtcNow,
                PreviousEventType = EventType.Started,
                Details = new MonitoringEventDetails
                {
                    { "some", "details" }
                }
            };
        }
    }
}
