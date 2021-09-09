using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
// using Polly; // Not used in Initial HostExtensions Creator  Code
// Polly Description: https://github.com/App-vNext/Polly
// Polly is a .NET resilience and transient-fault-handling library that allows developers to express policies such as Retry,
// Circuit Breaker, Timeout, Bulkhead Isolation, and Fallback in a fluent and thread-safe manner.
//  Project file reference: <PackageReference Include="Polly" Version="7.2.1" />


// Not used in GRPC, created to build and test Discount.Api ONLY
namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        // Initial HostExtensions Creator Code
        // ===========================
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postresql database.");

                    using var connection = new NpgsqlConnection
                        (configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    using var command = new NpgsqlCommand
                    {
                        Connection = connection
                    };

                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Migrated postresql database.");
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the postresql database");

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }
            }

            return host;
        }

        // 2nd Version of MigrateDatabase
        // ==============================
        //public static IHost MigrateDatabase<TContext>(this IHost host)
        //{
        //    using (var scope = host.Services.CreateScope())
        //    {
        //        var services = scope.ServiceProvider;
        //        var configuration = services.GetRequiredService<IConfiguration>();
        //        var logger = services.GetRequiredService<ILogger<TContext>>();

        //        try
        //        {
        //            logger.LogInformation("Migrating postressql database.");

        //            var retry = Policy.Handle<NpgsqlException>()
        //                    .WaitAndRetry(
        //                        retryCount: 5,
        //                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 2,4,8,16,32 sc
        //                        onRetry: (exception, retryCount, context) =>
        //                        {
        //                            logger.LogError($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
        //                        });

        //            //if the postgresql server container is not created on run docker compose this
        //            //migration can't fail for network related exception. The retry options for database operations
        //            //apply to transient exceptions                    
        //            retry.Execute(() => ExecuteMigrations(configuration));

        //            logger.LogInformation("Migrated postresql database.");
        //        }
        //        catch (NpgsqlException ex)
        //        {
        //            logger.LogError(ex, "An error occurred while migrating the postresql database");
        //        }
        //    }

        //    return host;
        //}

        // Called in 2nd version of MigrateDatabase
        //===========================================
        private static void ExecuteMigrations(IConfiguration configuration)
        {
            using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            connection.Open();

            using var command = new NpgsqlCommand
            {
                Connection = connection
            };

            // In the event you do not want to recreate the table, if it exists:
            // SELECT EXISTS (SELECT FROM pg_catalog.pg_class c JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace WHERE  n.nspname = 'public' AND    c.relname = 'coupon' );
            // should return true/false - but on sql server, you could get a -1
            // Then of course, you would not call the DROP TABLE
            command.CommandText = "DROP TABLE IF EXISTS Coupon";
            command.ExecuteNonQuery();

            command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)";
            command.ExecuteNonQuery();


            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);";
            command.ExecuteNonQuery();
        }        
    }
}
