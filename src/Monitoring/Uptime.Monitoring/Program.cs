using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Reliablesite.Service.Core;

namespace Uptime.Monitoring
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseDefaults<Startup>();
    }
}
