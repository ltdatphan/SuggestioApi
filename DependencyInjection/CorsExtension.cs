namespace SuggestioApi.DependencyInjection;

public static class CorsExtension
{
    public static IServiceCollection AddCorsConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder =>
                {
                    builder.WithOrigins(configuration["CORS:Origin"] ?? string.Empty) // Your React app URL
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        return services;
    }
}