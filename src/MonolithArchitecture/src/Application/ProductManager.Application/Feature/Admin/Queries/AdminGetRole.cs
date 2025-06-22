using ProductManager.Application.Common.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Repositories;

namespace ProductManager.Application.Feature.Admin.Queries;

public class AdminGetRoleQuery : IQuery<ApiResponse>
{
    public AdminGetRoleQuery(string roleName)
    {
        RoleName = roleName;
    }
    public string RoleName { get; set; }
}
internal class AdminGetRoleQueryHandler : IQueryHandler<AdminGetRoleQuery, ApiResponse>
{
    private readonly IAdminRepository _adminRepository;
    public AdminGetRoleQueryHandler(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }
    public async Task<ApiResponse> HandleAsync(AdminGetRoleQuery query, CancellationToken cancellationToken = default)
        => await _adminRepository.AdminGetRoleAsync(query.RoleName);
}
