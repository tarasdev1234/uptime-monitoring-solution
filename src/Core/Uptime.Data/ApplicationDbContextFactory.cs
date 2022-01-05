using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using System;
using Uptime.Data.Config;

namespace Uptime.Data {
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext> {
        public ApplicationDbContext CreateDbContext (string[] args) {
            //var services = new ServiceCollection();

            //services.Configure<DbSettings>(o => {
            //    o.DtabaseProvider = "SqlServer";
            //    o.ConnectionString = "Server=localhost\\SQLEXPRESS;Database=UptimeDev;Uid=sa;Password=superpass;MultipleActiveResultSets=true;";
            //});
            //services.AddDbContext<ApplicationDbContext>();

            //var provider = services.BuildServiceProvider();

            //var scope = provider.CreateScope();
            //var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            //return ctx;

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=uptime;Uid=sa;Password=yourStrong(!)Password;MultipleActiveResultSets=true;");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
