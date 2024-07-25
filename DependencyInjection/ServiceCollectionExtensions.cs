using Microsoft.AspNetCore.Authorization;
using SuggestioApi.Interfaces;
using SuggestioApi.Policies.Handlers;
using SuggestioApi.Repository;
using SuggestioApi.Service;

namespace SuggestioApi.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICuratedListRepository, CuratedListRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFollowRepository, FollowRepository>();
        services.AddScoped<IAuthorizationHandler, ListOwnerHandler>();
        services.AddScoped<IAuthorizationHandler, ItemOwnerHandler>();

        return services;
    }
}