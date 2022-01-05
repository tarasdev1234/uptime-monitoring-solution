using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Data.EntityConfigurations
{
    internal sealed class TemplatesConfiguration : IEntityTypeConfiguration<Template>
    {
        public void Configure(EntityTypeBuilder<Template> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Scope)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder.HasIndex(x => x.Scope);
            builder.HasIndex(x => x.Name);
        }
    }
}
