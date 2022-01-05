using System.Threading.Tasks;
using Uptime.Data.Models.Support;

namespace Uptime.Events.EventHandlers.Tickets {
    public interface ITicketEventsHandler {
        Task Created (Ticket ticket);

        Task StatusChanged (Ticket ticket, TicketStatus oldStatus);

        Task Assigned (Ticket ticket, long userId);

        Task MessageIn (Ticket ticket, TicketMessage message);

        Task MessageOut (Ticket ticket, TicketMessage message);

        Task Deleted (Ticket ticket);
    }
}
