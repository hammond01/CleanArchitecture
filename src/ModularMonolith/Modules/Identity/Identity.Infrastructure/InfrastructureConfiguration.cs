using System.Text;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure;

/// <summary>
/// Infrastructure layer configuration for Identity module
/// </summary>
public static class InfrastructureConfiguration
{
    public static IServiceCollection AddIdentityInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string connectionString)
    {
        // Register DbContext
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString,
                builder => builder.MigrationsHistoryTable("__EFMigrationsHistory", "identity")));

        // Register repository
        services.AddScoped<IIdentityRepository, IdentityRepository>();

        // Bind runtime settings from configuration
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<IdentitySecuritySettings>(configuration.GetSection("IdentitySecurity"));
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();

        if (jwtSettings.SecretKey.Length < 32)
        {
            throw new InvalidOperationException("JwtSettings:SecretKey must be at least 32 characters long.");
        }

        if (!environment.IsDevelopment()
            && !environment.IsEnvironment("Test")
            && string.Equals(jwtSettings.SecretKey, new JwtSettings().SecretKey, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "JwtSettings:SecretKey is using the development default. Override it before running outside Development/Test.");
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        // Register services
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddSingleton<IEmailTemplateProvider, EmailTemplateProvider>();
        services.AddScoped<IEmailService>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<EmailSettings>>().Value;
            if (settings.UseFakeEmail)
            {
                return new FakeEmailService(sp.GetRequiredService<ILogger<FakeEmailService>>());
            }

            return new SmtpEmailService(
                sp.GetRequiredService<IOptions<EmailSettings>>(),
                sp.GetRequiredService<ILogger<SmtpEmailService>>());
        });

        return services;
    }
}
