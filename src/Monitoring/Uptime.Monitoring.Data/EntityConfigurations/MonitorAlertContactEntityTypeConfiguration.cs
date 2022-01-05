using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Data.EntityConfigurations
{
    class MonitorAlertContactEntityTypeConfiguration : IEntityTypeConfiguration<MonitorAlertContact> {
        public void Configure (EntityTypeBuilder<MonitorAlertContact> builder) {
            builder.HasKey(mac => new { mac.MonitorId, mac.AlertContactId });
        }
    }
}
