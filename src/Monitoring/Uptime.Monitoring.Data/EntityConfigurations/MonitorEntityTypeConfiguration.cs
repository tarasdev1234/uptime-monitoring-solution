using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Data.EntityConfigurations
{
    class MonitorEntityTypeConfiguration : IEntityTypeConfiguration<Monitor> {
        public void Configure (EntityTypeBuilder<Monitor> builder) {
            builder
                .ToTable("Monitors")
                .HasDiscriminator(x => x.Type)
                .HasValue<HttpMonitor>(MonitorType.HTTP)
                .HasValue<PingMonitor>(MonitorType.PING)
                .HasValue<KeywordMonitor>(MonitorType.KEYWORD)
                .HasValue<TcpMonitor>(MonitorType.TCPPORT);

            builder.HasKey(m => m.Id);
            
            builder.Property(m => m.Name)
                .HasColumnName("Name")
                .IsRequired();

            builder.Property(m => m.Url)
                .HasColumnName("Url")
                .IsRequired();
        }
    }
}
