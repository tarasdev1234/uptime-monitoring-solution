using System.Threading.Tasks;
using Uptime.Monitoring.Model.Models;
using Uptime.Monitoring.Model.Abstractions;
using System.Collections.Generic;
using Uptime.Notifications.Model.Messages;
using Messaging;
using Uptime.Monitoring.Data;
using EventType = Uptime.Monitoring.Model.Models.EventType;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Dynamic;
using Newtonsoft.Json.Linq;

namespace Uptime.Monitoring.Logic.EventHandlers
{
    internal sealed class NotificationEventsHandler : ITaskEventsHandler
    {
        private readonly UptimeMonitoringContext dbContext;
        private readonly IProducer<NotificationMsg> producer;

        public NotificationEventsHandler (UptimeMonitoringContext dbContext, IProducer<NotificationMsg> producer)
        {
            this.dbContext = dbContext;
            this.producer = producer;
        }

        private async Task ProcessNotifications(MonitoringEvent @event, bool eventRepeated)
        {
            if (eventRepeated)
            {
                return;
            }

            (var monitor, var contacts) = await GetContacts(@event.MonitorId, @event.Type);

            var sendTasks = new List<Task>();

            var messages = CreateMessages(@event, contacts, monitor);
            foreach (var message in messages)
            {
                var task = producer.SendAsync(message);
                sendTasks.Add(task);
            }

            await Task.WhenAll(sendTasks);
        }

        public Task Start(Monitor monitor) => Task.CompletedTask;

        public Task Stop(Monitor monitor) => Task.CompletedTask;

        public Task Pause(Monitor monitor) => Task.CompletedTask;

        public Task Up(MonitorTask task, MonitoringEvent @event, bool eventRepeated) => ProcessNotifications(@event, eventRepeated);

        public Task Down(MonitorTask task, MonitoringEvent @event, bool eventRepeated) => ProcessNotifications(@event, eventRepeated);

        private async Task<(Monitor monitor, IEnumerable<AlertContact> contacts)> GetContacts(long id, EventType type)
        {
            var types = new List<int>() {
                (int)NotificationType.UPDOWN
            };

            switch (type)
            {
                case EventType.Up:
                    types.Add((int)NotificationType.ONLYUP);
                    break;
                case EventType.Down:
                    types.Add((int)NotificationType.ONLYDOWN);
                    break;
            }

            var contacts = await dbContext.Monitors
                .Where(m => m.Id == id)
                .Select(m => new
                {
                    Monitor = m,
                    Contacts = m.AlertContacts
                        .Select(ac => ac.AlertContact)
                        .Where(ac => ac.Active && types.Contains(ac.NotificationType))
                })
                .FirstOrDefaultAsync();

            return (contacts.Monitor, contacts.Contacts);
        }

        private IEnumerable<NotificationMsg> CreateMessages(MonitoringEvent @event, IEnumerable<AlertContact> contacts, Monitor monitor)
        {
            var result = new List<NotificationMsg>();

            var eventType = @event.Type switch
            {
                EventType.Up => "Up",
                EventType.Down => "Down",
                _ => "Unknown"
            };

            foreach (var group in contacts.GroupBy(x => x.Type))
            {
                var deliveryType = group.Key switch
                {
                    (int)ContactType.EMAIL => DeliveryType.Email,
                    _ => DeliveryType.Unknown
                };

                dynamic data = new ExpandoObject();
                data.MonitorName = monitor.Name;
                data.Details = @event.Details;

                result.Add(new NotificationMsg("Monitoring", eventType)
                {
                    DeliveryType = deliveryType,
                    Destinations = group.Select(x => x.Email),
                    Data = data
                });
            }

            return result;
        }
    }
}
