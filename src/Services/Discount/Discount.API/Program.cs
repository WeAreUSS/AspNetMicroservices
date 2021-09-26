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
                    // The below is an optional way to configure KestrelListners
                    // When you set the URLs for kestrel in this way,
                    // it overrides the URLS configuration value if you've set it through one of the other mechanisms as well,
                    // such as environment variables.
                    //==========================================================
                    //webBuilder.UseKestrel(opts =>
                    //{
                    //    // Bind directly to a socket handle or Unix socket
                    //    // opts.ListenHandle(123554);
                    //    // opts.ListenUnixSocket("/tmp/kestrel-test.sock");
                    //    opts.Listen(IPAddress.Loopback, port: 5002);
                    //    opts.ListenAnyIP(5003);
                    //    opts.ListenLocalhost(5004, opts => opts.UseHttps());
                    //    opts.ListenLocalhost(5005, opts => opts.UseHttps());
                    //});
                });    

      
    }
}
