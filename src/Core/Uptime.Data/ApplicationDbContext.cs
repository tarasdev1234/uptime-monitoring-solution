using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Uptime.Data.Config;
using Uptime.Data.Models;
using Uptime.Data.Models.Billing;
using Uptime.Data.Models.Branding;
using Uptime.Data.Models.Identity;
using Uptime.Data.Models.KnowledgeBase;
using Uptime.Data.Models.Plugins;
using Uptime.Data.Models.Support;

namespace Uptime.Data {
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken> {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger log;

        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base(options) { }

        //public ApplicationDbContext (IServiceProvider sp, ILogger<ApplicationDbContext> logger) {
        //    serviceProvider = sp;
        //    log = logger;
        //}

        //protected override void OnConfiguring (DbContextOptionsBuilder options) {
        //    base.OnConfiguring(options);

        //    options.UseSqlServer(settings.Value.ConnectionString,
        //        sqlServerOptionsAction: sqlOptions => {
        //            if (!string.IsNullOrEmpty(settings.Value.MigrationsAssembly)) {
        //                sqlOptions.MigrationsAssembly(settings.Value.MigrationsAssembly);
        //            }

        //            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        //        });
        //}

        //protected override void OnModelCreating (ModelBuilder modelBuilder) {
        //    base.OnModelCreating(modelBuilder);

        //    foreach (var entityDescriptor in serviceProvider.GetServices<IEntityDescriptor>()) {
        //        entityDescriptor.OnModelCreating(modelBuilder);
        //    }
        //    //Core.AppHost.Instance.ConfigureEntities(modelBuilder);
        //}

        protected override void OnModelCreating (ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<UserDepartment>()
                .HasKey(c => new { c.UserId, c.DepartmentId });

            builder.Entity<KbArticleBrand>()
                .HasKey(c => new { c.ArticleId, c.BrandId });

            builder.Entity<BrandPlugin>()
                .HasKey(c => new { c.Name, c.BrandId });

            builder.Entity<KbArticleTag>()
                .HasKey(c => new { c.ArticleId, c.TagId });

            builder.Entity<KbArticle>()
                .HasIndex(kba => kba.Slug)
                .IsUnique();

            builder.Entity<CompanySettings>()
                .HasKey(c => new { c.CompanyId, c.UserId });

            builder.Entity<CompanySettings>()
                .HasIndex(cs => cs.CompanyId)
                .IsUnique(false);

            builder.Entity<CompanySettings>()
                .HasIndex(cs => cs.UserId)
                .IsUnique(true);

            builder.Entity<CompanySettings>()
                .HasIndex(c => new { c.CompanyId, c.UserId, c.IsOwner })
                .IsUnique(true);

            builder.Entity<ApplicationRole>()
                .HasMany(r => r.Claims)
                .WithOne()
                .HasForeignKey(c => c.RoleId);

            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("Money");

            builder.Entity<KbArticle>()
                .HasMany(r => r.Comments)
                .WithOne(c => c.Article)
                .HasForeignKey(c => c.ArticleId);

            builder.Entity<KbComment>()
                .HasOne(c => c.Article)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.ArticleId);

            builder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId);

            builder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<ApplicationRole>().ToTable("Roles");
            builder.Entity<RoleClaim>().ToTable("RoleClaims");
            builder.Entity<UserClaim>().ToTable("UserClaims");
            builder.Entity<UserLogin>().ToTable("UserLogins");
            builder.Entity<UserRole>().ToTable("UserRoles");
            builder.Entity<UserToken>().ToTable("UserTokens");

            builder.Entity<Brand>().ToTable("Brands");
            builder.Entity<Department>().ToTable("Departments");
            builder.Entity<UserDepartment>().ToTable("UserDepartments");
            builder.Entity<Company>().ToTable("Companies");
            builder.Entity<CompanySettings>().ToTable("CompanySettings");
            builder.Entity<Currency>().ToTable("Currencies");
            builder.Entity<CurrencyFormat>().ToTable("CurrencyFormats");
            builder.Entity<PaymentGateway>().ToTable("PaymentGateways");
            builder.Entity<Product>().ToTable("Products");
            builder.Entity<ProductGroup>().ToTable("ProductGroups");
            builder.Entity<KbArticle>().ToTable("KbArticles");
            builder.Entity<KbSeries>().ToTable("KbSeries");
            builder.Entity<KbTag>().ToTable("KbTags");
            builder.Entity<KbArticleBrand>().ToTable("KbArticleBrands");
            builder.Entity<KbComment>().ToTable("KbComments");
            builder.Entity<AutoResponder>().ToTable("AutoResponders");
            builder.Entity<PopServer>().ToTable("PopServers");
            builder.Entity<SmtpServer>().ToTable("SmtpServers");
            builder.Entity<Ticket>().ToTable("Tickets");
            builder.Entity<TicketMessage>().ToTable("TicketMessages");
            builder.Entity<TicketStatus>().ToTable("TicketStatuses");
            builder.Entity<TicketEvent>().ToTable("TicketEvents");
            builder.Entity<TicketComment>().ToTable("TicketComments");
            builder.Entity<CommentType>().ToTable("CommentTypes");
            builder.Entity<KbCategory>().ToTable("KbCategories");
            builder.Entity<SecureObject>().ToTable("SecureObjects");
            builder.Entity<PermissionType>().ToTable("PermissionTypes");
            builder.Entity<UserPermission>().ToTable("UserPermissions");
            builder.Entity<Theme>().ToTable("Themes");
            builder.Entity<Plugin>().ToTable("Plugins");
            builder.Entity<BrandPlugin>().ToTable("BrandPlugins");
        }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<UserDepartment> UserDepartments { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<CompanySettings> CompanySettings { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        public DbSet<CurrencyFormat> CurrencyFormats { get; set; }

        public DbSet<PaymentGateway> PaymentGateways { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductGroup> ProductGroups { get; set; }

        public DbSet<KbArticle> KbArticles { get; set; }

        public DbSet<KbSeries> KbSeries { get; set; }

        public DbSet<KbTag> KbTags { get; set; }

        public DbSet<KbArticleTag> KbArticleTags { get; set; }

        public DbSet<KbArticleBrand> KbArticleBrands { get; set; }

        public DbSet<KbComment> KbComments { get; set; }

        public DbSet<AutoResponder> AutoResponders { get; set; }

        public DbSet<PopServer> PopServers { get; set; }

        public DbSet<SmtpServer> SmtpServers { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<TicketMessage> TicketMessages { get; set; }

        public DbSet<TicketStatus> TicketStatuses { get; set; }

        public DbSet<TicketEvent> TicketEvents { get; set; }

        public DbSet<TicketComment> TicketComments { get; set; }

        public DbSet<CommentType> CommentTypes { get; set; }

        public DbSet<KbCategory> KbCategories { get; set; }

        public DbSet<SecureObject> SecureObjects { get; set; }

        public DbSet<PermissionType> PermissionTypes { get; set; }

        public DbSet<UserPermission> UserPermissions { get; set; }

        public DbSet<Theme> Themes { get; set; }

        public DbSet<Plugin> Plugins { get; set; }

        public DbSet<BrandPlugin> BrandPlugins { get; set; }
    }
}