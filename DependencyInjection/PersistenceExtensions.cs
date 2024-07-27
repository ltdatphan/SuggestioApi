using System.Web;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SuggestioApi.Data;

namespace SuggestioApi.DependencyInjection;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var connectionString = string.Empty;

        if (environment.IsDevelopment())
        {
            connectionString = configuration.GetConnectionString("Sql");
        }
        else
        {
            // // Use connection string provided at runtime by Fly.
            // var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            //
            // // Parse connection URL to connection string for Npgsql
            // connUrl = connUrl.Replace("postgres://", string.Empty);
            // var pgUserPass = connUrl.Split("@")[0];
            // var pgHostPortDb = connUrl.Split("@")[1];
            // var pgHostPort = pgHostPortDb.Split("/")[0];
            // var pgDb = pgHostPortDb.Split("/")[1];
            // var pgUser = pgUserPass.Split(":")[0];
            // var pgPass = pgUserPass.Split(":")[1];
            // var pgHost = pgHostPort.Split(":")[0];
            // var pgPort = pgHostPort.Split(":")[1];
            // var updatedHost = pgHost.Replace("flycast", "internal");

            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (!string.IsNullOrEmpty(databaseUrl))
            {
                var dbUri = new Uri(databaseUrl);
                var userInfo = dbUri.UserInfo.Split(':');
                var query = HttpUtility.ParseQueryString(dbUri.Query);
                var sslMode = query.Get("sslmode") ?? "Require";

                var connectionStringBuilder = new NpgsqlConnectionStringBuilder
                {
                    Host = dbUri.Host,
                    Port = dbUri.Port,
                    Username = userInfo[0],
                    Password = userInfo[1],
                    Database = dbUri.LocalPath.TrimStart('/'),
                    SslMode = sslMode.Equals("disable", StringComparison.OrdinalIgnoreCase)
                        ? SslMode.Disable
                        : SslMode.Require,
                    TrustServerCertificate = sslMode.Equals("disable", StringComparison.OrdinalIgnoreCase)
                };

                connectionString = connectionStringBuilder.ToString();
            }
        }

        ;

        services.AddDbContext<ApplicationDBContext>(options => { options.UseNpgsql(connectionString); });

        return services;
    }
}