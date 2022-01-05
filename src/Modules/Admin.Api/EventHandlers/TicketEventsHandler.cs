using System;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Data.Models.Support;
using Uptime.Events;
using Uptime.Events.EventHandlers.Tickets;

namespace Admin.Api.EventHandlers {
    public class TicketEventsHandler : ITicketEventsHandler {
        private readonly ApplicationDbContext dbContext;

        public TicketEventsHandler (ApplicationDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public async Task Assigned (Ticket ticket, long userId) {
            dbContext.TicketEvents.Add(new TicketEvent() {
                Type = TicketEvents.ASSIGNED,
                Date = DateTime.Now,
                TicketId = ticket.Id
            });

            await dbContext.SaveChangesAsync();
        }

        public async Task Created (Ticket ticket) {
            dbContext.TicketEvents.Add(new TicketEvent() {
                Type = TicketEvents.CREATED,
                Date = DateTime.Now,
                TicketId = ticket.Id
            });

            await dbContext.SaveChangesAsync();
        }

        public async Task Deleted (Ticket ticket) {
            dbContext.TicketEvents.Add(new TicketEvent() {
                Type = TicketEvents.DELETED,
                Date = DateTime.Now,
                TicketId = ticket.Id
            });

            await dbContext.SaveChangesAsync();
        }

        public async Task MessageIn (Ticket ticket, TicketMessage message) {
            dbContext.TicketEvents.Add(new TicketEvent() {
                Type = TicketEvents.MESSAGE_IN,
                Date = DateTime.Now,
                TicketId = ticket.Id
            });

            await dbContext.SaveChangesAsync();
        }

        public async Task MessageOut (Ticket ticket, TicketMessage message) {
            dbContext.TicketEvents.Add(new TicketEvent() {
                Type = TicketEvents.MESSAGE_OUT,
                Date = DateTime.Now,
                TicketId = ticket.Id
            });

            await dbContext.SaveChangesAsync();
        }

        public async Task StatusChanged (Ticket ticket, TicketStatus oldStatus) {
            dbContext.TicketEvents.Add(new TicketEvent() {
                Type = TicketEvents.STATUS_CHANGED,
                Date = DateTime.Now,
                TicketId = ticket.Id
            });

            await dbContext.SaveChangesAsync();
        }
    }
}
