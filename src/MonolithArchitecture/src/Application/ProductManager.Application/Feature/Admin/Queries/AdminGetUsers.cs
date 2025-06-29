using ProductManager.Application.Common.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Repositories;

namespace ProductManager.Application.Feature.Admin.Queries;

public class AdminGetUsersQuery : IQuery<ApiResponse>
{
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 0;
}
internal class AdminGetUsersQueryHandler : IQueryHandler<AdminGetUsersQuery, ApiResponse>
{
    private readonly IAdminRepository _adminRepository;
    public AdminGetUsersQueryHandler(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }
    public async Task<ApiResponse> HandleAsync(AdminGetUsersQuery query, CancellationToken cancellationToken = default)
        => await _adminRepository.AdminGetUsers(query.PageSize, query.PageNumber);
}
