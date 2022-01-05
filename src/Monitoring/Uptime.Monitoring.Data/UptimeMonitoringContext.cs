using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Uptime.Monitoring.Data.EntityConfigurations;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Data
{
    public class UptimeMonitoringContext : DbContext {
        public UptimeMonitoringContext (DbContextOptions<UptimeMonitoringContext> options) : base(options) { }

        public DbSet<Monitor> Monitors { get; set; }

        public DbSet<UserSettings> UserSettings { get; set; }

        public DbSet<SupportTicket> SupportTicket { get; set; }

        public DbSet<AlertContact> AlertContacts { get; set; }

        public DbSet<MonitorAlertContact> MonitorAlertContacts { get; set; }

        public DbSet<MonitoringServer> MonitoringServers { get; set; }

        

        protected override void OnModelCreating (ModelBuilder builder) {
            builder.ApplyConfiguration(new MonitorEntityTypeConfiguration());
            builder.ApplyConfiguration(new MonitorAlertContactEntityTypeConfiguration());
        }
    }

    public class UptimeMonitoringContextDesignFactory : IDesignTimeDbContextFactory<UptimeMonitoringContext>
    {
        public UptimeMonitoringContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UptimeMonitoringContext>()
                .UseSqlServer("Server=db1.ny1.uptime.engineer;Database=uptime.monitoring;User=ue-thaidar;Password=3kVyDWGE62R7pnJq;MultipleActiveResultSets=true;");
                //.UseSqlServer("Server=DESKTOP-VF5C5O5\\SQLEXPRESS;Database=uptime.monitoring;User=sa;Password=Password!@#;MultipleActiveResultSets=true;");

            return new UptimeMonitoringContext(optionsBuilder.Options);
        }
    }
}
