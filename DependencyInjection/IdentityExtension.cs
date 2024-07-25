using Microsoft.AspNetCore.Identity;
using SuggestioApi.Data;
using SuggestioApi.Models;

namespace SuggestioApi.DependencyInjection;

public static class IdentityExtension
{
    public static IServiceCollection AddUserIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 12;

            //Lockout options
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //Lockout for 5 minutes
            options.Lockout.MaxFailedAccessAttempts = 3;
        }).AddEntityFrameworkStores<ApplicationDBContext>();

        return services;
    }
}