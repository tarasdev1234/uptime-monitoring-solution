using Microsoft.EntityFrameworkCore;
using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Data
{
    public sealed class UptimeNotificationsContext : DbContext
    {
        public UptimeNotificationsContext(DbContextOptions<UptimeNotificationsContext> options) : base(options)
        {
        }

        public DbSet<Template> Templates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UptimeNotificationsContext).Assembly);
        }
    }
}
