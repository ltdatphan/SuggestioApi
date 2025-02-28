using SuggestioApi.Infrastructure;

namespace SuggestioApi.DependencyInjection;

public static class ExceptionExtension
{
    public static IServiceCollection AddExceptions(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            };
        });
        return services;
    }
}