using Microsoft.EntityFrameworkCore;
using SuggestioApi.Data;

namespace SuggestioApi.DependencyInjection;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        string connectionString = String.Empty;
        
        if (environment.IsDevelopment())
            connectionString = configuration.GetConnectionString("Sql");
        else
        {
            // Use connection string provided at runtime by Fly.
            var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    
            // Parse connection URL to connection string for Npgsql
            connUrl = connUrl.Replace("postgres://", string.Empty);
            var pgUserPass = connUrl.Split("@")[0];
            var pgHostPortDb = connUrl.Split("@")[1];
            var pgHostPort = pgHostPortDb.Split("/")[0];
            var pgDb = pgHostPortDb.Split("/")[1];
            var pgUser = pgUserPass.Split(":")[0];
            var pgPass = pgUserPass.Split(":")[1];
            var pgHost = pgHostPort.Split(":")[0];
            var pgPort = pgHostPort.Split(":")[1];
            var updatedHost = pgHost.Replace("flycast", "internal");

            connectionString = $"Server={updatedHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};";
        };
        
        services.AddDbContext<ApplicationDBContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}