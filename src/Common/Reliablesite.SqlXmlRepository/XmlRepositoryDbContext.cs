using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Xml.Linq;

namespace Reliablesite.SqlXmlRepository
{
    public sealed class XmlRepositoryDbContext : DbContext
    {
        public XmlRepositoryDbContext(DbContextOptions<XmlRepositoryDbContext> options) : base(options)
        {
        }

        public DbSet<XmlRepositoryEntry> DataProtectionKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var xmlEntryBuilder = modelBuilder
                .Entity<XmlRepositoryEntry>()
                .ToTable("DataProtectionKeys");

            xmlEntryBuilder
                .HasKey(x => x.Id);

            xmlEntryBuilder
                .Property(x => x.Value)
                .HasConversion(
                    x => x.ToString(SaveOptions.DisableFormatting),
                    x => XElement.Parse(x));

            xmlEntryBuilder
                .Property(x => x.FriendlyName)
                .HasMaxLength(255);
        }
    }

    internal class XmlRepositoryDbContextFactoty : IDesignTimeDbContextFactory<XmlRepositoryDbContext>
    {
        public XmlRepositoryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<XmlRepositoryDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=localhost;Database=design;Uid=sa;Password=Qwerty123;MultipleActiveResultSets=true;",
                sqlOptions => sqlOptions.MigrationsHistoryTable("__XmlRepositoryMigrationHistory"));

            return new XmlRepositoryDbContext(optionsBuilder.Options);
        }
    }
}
