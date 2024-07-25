using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SuggestioApi.Policies.Requirements;

namespace SuggestioApi.DependencyInjection;

public static class AuthExtension
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "accessToken";
                options.Cookie.HttpOnly = true; // Prevent access to the cookie via JavaScript
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure cookies are only sent over HTTPS
                options.ExpireTimeSpan = TimeSpan.FromMinutes(15); // Set expiration time
                // options.SlidingExpiration = true; // Renew the cookie with each request
                options.Cookie.SameSite = SameSiteMode.None; // Adjust SameSite attribute for security
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JWT:SigningKey"] ?? string.Empty)
                    )
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["accessToken"];
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(
            options =>
            {
                options.AddPolicy("ListOwner", policy => policy.Requirements.Add(new ListOwnerRequirement()));
                options.AddPolicy("ItemOwner", policy => policy.Requirements.Add(new ItemOwnerRequirement()));
            }
        );

        return services;
    }
}