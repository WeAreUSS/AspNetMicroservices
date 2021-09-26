using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Discount.Grpc.Extensions;

namespace Discount.Grpc
{
    public class Program
    {
        public static void Main(string[] args)
        {
           // CreateHostBuilder(args).Build().Run(); <-- Default code

            // The following was copied from Discount.API Program.cs
            var host = CreateHostBuilder(args).Build();
            // Reference: Discount.Grpc.Extensions (also copied from Discount.API)
            host.MigrateDatabase<Program>(); // <-- Method is in Host\HostExtensions.cs , 
                                             // This is a seeding and table creation method for the Postgre database.
                                             // It basically drops and creates the Coupon discounts Table and installs a few discounts into that table
            host.Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
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
