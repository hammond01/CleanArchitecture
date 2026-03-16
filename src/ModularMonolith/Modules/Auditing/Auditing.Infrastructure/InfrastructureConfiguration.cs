using Auditing.Domain.Repositories;
using Auditing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Auditing.Infrastructure;

/// <summary>
/// Infrastructure layer configuration for Auditing module
/// </summary>
public static class InfrastructureConfiguration
{
    public static IServiceCollection AddAuditingInfrastructureServices(
        this IServiceCollection services,
        string connectionString)
    {
        // Register DbContext
        services.AddDbContext<AuditingDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register repository
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        return services;
    }
}
