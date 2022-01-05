using System.Collections.Generic;
using Uptime.Data.Models.Support;

namespace Uptime.Data.Repositories.Support {
    public interface ITicketRepository : IBaseRepository<Ticket> {
        IEnumerable<Ticket> GetAll ();
    }
}
