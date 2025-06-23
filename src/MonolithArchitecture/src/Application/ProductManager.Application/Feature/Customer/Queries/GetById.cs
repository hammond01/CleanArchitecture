using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
namespace ProductManager.Application.Feature.Customer.Queries;

public record GetCustomerByIdQuery : IQuery<ApiResponse>
{
    public GetCustomerByIdQuery(string customerId)
    {
        CustomerId = customerId;
    }
    public string CustomerId { get; set; }
}
public class GetCustomerByIdHandler : IQueryHandler<GetCustomerByIdQuery, ApiResponse>
{
    private readonly ICrudService<Customers> _customerService;

    public GetCustomerByIdHandler(ICrudService<Customers> customerService)
    {
        _customerService = customerService;
    }

    public async Task<ApiResponse> HandleAsync(GetCustomerByIdQuery query, CancellationToken cancellationToken = default)
    {
        var response = await _customerService.GetByIdAsync(query.CustomerId);
        return response == null ? new ApiResponse(404, "Customer not found")
            : new ApiResponse(200, "Get customer successfully", response);
    }
}
