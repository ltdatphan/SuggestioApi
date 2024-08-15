namespace SuggestioApi.DependencyInjection;

public static class CookiePolicyExtension
{
    public static IServiceCollection ConfigureCookiePolicy(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Lax; // or other as required
            options.OnAppendCookie = cookieContext =>
                AppendDomainToCookie(cookieContext.CookieOptions, configuration["CustomDomain"]);
            options.OnDeleteCookie = cookieContext =>
                AppendDomainToCookie(cookieContext.CookieOptions, configuration["CustomDomain"]);
        });

        return services;
    }

    private static void AppendDomainToCookie(CookieOptions options, string? customDomain)
    {
        if (!string.IsNullOrEmpty(customDomain)) options.Domain = customDomain;
    }
}