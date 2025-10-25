using IdentityServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants.Permissions;

namespace IdentityServer.Infrastructure.Data;

public class OpenIddictSeeder
{
    private readonly IServiceProvider _serviceProvider;

    public OpenIddictSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync()
    {
        using var scope = _serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        // Create a client application for testing (password grant + authorization code)
        if (await manager.FindByClientIdAsync("postman-client") == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "postman-client",
                ClientSecret = "postman-secret",
                DisplayName = "Postman Test Client",
                Permissions =
                {
                    Endpoints.Token,
                    Endpoints.Authorization,
                    Endpoints.Introspection,
                    Endpoints.Revocation,

                    GrantTypes.Password,
                    GrantTypes.RefreshToken,
                    GrantTypes.AuthorizationCode,
                    GrantTypes.ClientCredentials,

                    ResponseTypes.Code,

                    Scopes.Email,
                    Scopes.Profile,
                    Scopes.Roles
                },
                RedirectUris =
                {
                    new Uri("https://oauth.pstmn.io/v1/callback"),
                    new Uri("http://localhost:5000/callback"),
                    new Uri("https://localhost:5001/callback")
                },
                PostLogoutRedirectUris =
                {
                    new Uri("http://localhost:5000/"),
                    new Uri("https://localhost:5001/")
                }
            });
        }

        // Create a client for Swagger UI
        if (await manager.FindByClientIdAsync("swagger-client") == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "swagger-client",
                DisplayName = "Swagger UI",
                Permissions =
                {
                    Endpoints.Token,
                    Endpoints.Authorization,

                    GrantTypes.AuthorizationCode,
                    GrantTypes.RefreshToken,

                    ResponseTypes.Code,

                    Scopes.Email,
                    Scopes.Profile,
                    Scopes.Roles
                },
                RedirectUris =
                {
                    new Uri("http://localhost:5218/swagger/oauth2-redirect.html"),
                    new Uri("https://localhost:5219/swagger/oauth2-redirect.html")
                }
            });
        }

        // Seed scopes
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        if (await scopeManager.FindByNameAsync("api") == null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "api",
                DisplayName = "API Access",
                Resources = { "resource_server" }
            });
        }
    }
}
