using System.Collections.Generic;
using System.Linq;
using Uptime.Data.Models.Support;

namespace Uptime.Data.Repositories.Support {
    public class TicketRepository : BaseRepository<Ticket>, ITicketRepository {
        public TicketRepository (ApplicationDbContext dbContext) : base(dbContext) { }

        public IEnumerable<Ticket> GetAll () {
            return dbSet.ToList();
        }
    }
}
