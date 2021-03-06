using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Discount.API.Repositories;
using Discount.API.Repositories.Interfaces;

using Infrastructure.ServiceDiscovery;

namespace Discount.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Added for repository injection in controller
            services.AddScoped<IDiscountRepository, DiscountRepository>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Discount.API", Version = "v1" });
            });

            //services.AddHealthChecks()
            //    .AddNpgSql(Configuration["DatabaseSettings:ConnectionString"]);
            ConfigureConsul(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Discount.API v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            // First Version
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                //{
                //    Predicate = _ => true,
                //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                //});
            });
        }

        #region Private Methods

        //private void ConfigureJWT(IServiceCollection services)
        //{
        //    services.AddAuthentication("Bearer")
        //        .AddJwtBearer("Bearer", options =>
        //        {
        //            options.Authority = Configuration["IdentityServer:BaseUrl"];
        //            options.TokenValidationParameters = new TokenValidationParameters
        //            {
        //                ValidateAudience = false
        //            };
        //        });

        //    services.AddAuthorization(options =>
        //    {
        //        options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "shop_mvc_client"));
        //    });
        //}

        private void ConfigureConsul(IServiceCollection services)
        {
            var serviceConfig = Configuration.GetServiceConfig();

            services.RegisterConsulServices(serviceConfig);
        } 

        #endregion  Private Methods
    }
}
