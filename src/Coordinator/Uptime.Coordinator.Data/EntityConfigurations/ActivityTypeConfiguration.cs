using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uptime.Coordinator.Model.Models;

namespace Uptime.Coordinator.Data.EntityConfigurations
{
    internal sealed class ActivityTypeConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.ToTable("Activities");

            builder.HasKey(x => x.MonitorId);
            builder.Property(x => x.MonitorId).ValueGeneratedNever();
            builder.HasIndex(x => x.CoordinatorId);
            builder.HasIndex(x => x.ExecutorId);

            builder
                .Property(x => x.RowVersion)
                .HasDefaultValue(0)
                .IsRowVersion();
        }
    }
}
