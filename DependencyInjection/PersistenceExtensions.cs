using Microsoft.EntityFrameworkCore;
using SuggestioApi.Data;

namespace SuggestioApi.DependencyInjection;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDBContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Sql"));
        });

        return services;
    }
}