//using Common.Logging;

using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//using Serilog;

namespace OcelotApiGw
{
    public class Program
    {
        public static void Main(string[] args)
        {
           CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                   // dynamically select filename based upon environmental variable for environment.
                    config.AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
                })
                .ConfigureLogging((hostingContext, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddDebug();
                })
                //.UseSerilog(SeriLogger.Configure)
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
