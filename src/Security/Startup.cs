using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // The following adds Quickstart functionality to IS4
            //====================================================
            services.AddControllersWithViews();
            ConfigureIdentityServer(services);

            //services.AddIdentityServer()
            //    .AddInMemoryClients(Config.Clients)
            //    .AddInMemoryApiScopes(Config.ApiScopes)
            //    .AddInMemoryIdentityResources(Config.IdentityResources)
            //    .AddTestUsers(TestUsers.Users)
            //    .AddDeveloperSigningCredential();// <-- This is an automatically generated credential for development and testing purposes
            //                                     // This credential is linked to a temporary certificate: "tempkey.jwk" file for developer access purposes.
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }

            // The following allows for .css files, etc.. from the wwwroot folder to be   
            // used for Quickstart functionality to IS4
            //====================================================
            app.UseStaticFiles();

            app.UseRouting();
          
            app.UseIdentityServer();

            // allows for navigation to IS4 LogIn page
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                app.UseEndpoints(endpoints =>
                {
                    // Changed to allow for IS4 Quickstart UI functionality 
                    endpoints.MapDefaultControllerRoute();
                });
            });


            // For Console use
            //====================
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Identity Server is Working...");
            //    });
            //});

        }
        #region Private Methods
        private void ConfigureIdentityServer(IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddInMemoryClients(Config.Clients(Configuration))
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddTestUsers(TestUsers.Users)
                .AddDeveloperSigningCredential();
        } 
        #endregion
    }
}
