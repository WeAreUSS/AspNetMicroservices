using EventBus.Messages.Common;
using Infrastructure.ServiceDiscovery;
//using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ordering.API.EventBusConsumer;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;

namespace Ordering.API
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
            services.AddApplicationServices();
            services.AddInfrastructureServices(Configuration);

            // MassTransit-RabbitMQ Configuration
            //=======================================
            services.AddMassTransit(config =>
            {

                config.AddConsumer<BasketCheckoutConsumer>(); // we are stating that this is a consumer of RabbitMQ via MassTransit

                // MassTransit-RabbitMQ Configuration
                // ==================================
                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(Configuration["EventBusSettings:HostAddress"]);
                   // cfg.UseHealthCheck(ctx);

                   // below is the actual subscription and direction as to where the message should go(routing within the application)
                   // BasketCheckoutQueue in defined in: BuildingBlocks.EventBus.Messages.Common
                    cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
                    {
                        c.ConfigureConsumer<BasketCheckoutConsumer>(ctx); // here we direct the consumption to BasketCheckoutConsumer
                    });
                });
            });
            services.AddMassTransitHostedService();
            services.AddScoped<BasketCheckoutConsumer>(); //MassTransit &  RabbitMQ Injection of handler


            // General Configuration
            //============================
            services.AddAutoMapper(typeof(Startup));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering.API", Version = "v1" });
            });

            //services.AddHealthChecks()
            //        .AddDbContextCheck<OrderContext>();

            ConfigureJWT(services);
            ConfigureConsul(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                //{
                //    Predicate = _ => true,
                //    //ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                //});
            });
        }

        #region Private Methods

        private void ConfigureJWT(IServiceCollection services)
        {
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration["IdentityServer:BaseUrl"];
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "shop_mvc_client"));
            });
        }

        private void ConfigureConsul(IServiceCollection services)
        {
            var serviceConfig = Configuration.GetServiceConfig();

            services.RegisterConsulServices(serviceConfig);
        } 

        #endregion  Private Methods
    }
}
