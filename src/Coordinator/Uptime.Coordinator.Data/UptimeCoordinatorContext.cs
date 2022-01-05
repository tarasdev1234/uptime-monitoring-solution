using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Uptime.Coordinator.Model.Models;

namespace Uptime.Coordinator.Data
{
    public sealed class UptimeCoordinatorContext : DbContext
    {
        public UptimeCoordinatorContext(DbContextOptions<UptimeCoordinatorContext> options) : base(options)
        {
        }

        public DbSet<Activity> Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UptimeCoordinatorContext).Assembly);
        }
    }
}
