using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Discount.API.Extensions;

namespace Discount.API
{
    public class Program
    {
        public static void Main(string[] args)
        {

            // The following was the original code
            // ======================================
            // CreateHostBuilder(args).Build().Run();

            var host = CreateHostBuilder(args).Build();
            // Reference: Discount.API.Extensions
            host.MigrateDatabase<Program>();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.UseSerilog(SeriLogger.Configure) // Added for Serilog
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });    

      
    }
}
