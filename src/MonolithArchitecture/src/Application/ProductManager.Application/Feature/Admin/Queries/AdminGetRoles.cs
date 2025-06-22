using ProductManager.Application.Common.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Repositories;

namespace ProductManager.Application.Feature.Admin.Queries;

public class AdminGetRolesQuery : IQuery<ApiResponse>
{
    public int PageSize { get; set; } = 0;
    public int PageNumber { get; set; } = 0;
}
internal class AdminGetRolesHandler : IQueryHandler<AdminGetRolesQuery, ApiResponse>
{
    private readonly IAdminRepository _adminRepository;
    public AdminGetRolesHandler(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }
    public async Task<ApiResponse> HandleAsync(AdminGetRolesQuery query, CancellationToken cancellationToken = default)
        => await _adminRepository.AdminGetRolesAsync(query.PageSize, query.PageNumber);
}
