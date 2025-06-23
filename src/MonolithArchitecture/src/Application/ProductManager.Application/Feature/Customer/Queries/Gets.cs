using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
namespace ProductManager.Application.Feature.Customer.Queries;

public record GetCustomers : IQuery<ApiResponse>;
internal class GetCustomersHandler : IQueryHandler<GetCustomers, ApiResponse>
{
    private readonly ICrudService<Customers> _customerService;

    public GetCustomersHandler(ICrudService<Customers> customerService)
    {
        _customerService = customerService;
    }

    public async Task<ApiResponse> HandleAsync(GetCustomers query, CancellationToken cancellationToken = default)
    {
        var response = await _customerService.GetAsync();
        return new ApiResponse(200, "Get customers successfully", response);
    }
}
