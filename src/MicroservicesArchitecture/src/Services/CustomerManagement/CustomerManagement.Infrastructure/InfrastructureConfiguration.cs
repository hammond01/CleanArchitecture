using Microsoft.Extensions.DependencyInjection;
using CustomerManagement.Domain.Services;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Infrastructure.Repositories;

namespace CustomerManagement.Infrastructure;

public static class InfrastructureConfiguration
{
  public static IServiceCollection AddCustomerManagementInfrastructure(this IServiceCollection services)
  {
    // Register repositories
    services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

    return services;
  }
}
