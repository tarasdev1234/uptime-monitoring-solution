using Microsoft.EntityFrameworkCore;

namespace Uptime.Data {
    public interface IEntityDescriptor {
        void OnModelCreating (ModelBuilder modelBuilder);
    }
}
