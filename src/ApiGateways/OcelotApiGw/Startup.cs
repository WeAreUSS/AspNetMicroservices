using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Cache.CacheManager; // not initially used
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

namespace OcelotApiGw
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
        public void ConfigureServices(IServiceCollection services ) //, IConfiguration configuration
        {

            ConfigureAuthentication(services);
            ConfigureOcelot(services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
          
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Ocelot Api Gateway Working...");
                });
            });

            await app.UseOcelot();
        }


        #region Private Methods
        private void ConfigureAuthentication(IServiceCollection services)
        {
            var authenticationProviderKey = "IdentityApiKey";
            services.AddAuthentication()
                .AddJwtBearer(authenticationProviderKey, x =>
                {
                    x.Authority = Configuration["IdentityServer:BaseUrl"]; // IDENTITY SERVER URL
                    //x.RequireHttpsMetadata = false;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });
        } 

        private void ConfigureOcelot(IServiceCollection services)
        {
           
            services.AddOcelot()
                //        .AddCacheManager(settings => settings.WithDictionaryHandle()); // not added initially
                .AddCacheManager(x =>
                {
                    x.WithDictionaryHandle();
                })
                .AddConsul();
        }
        #endregion
    }
}
