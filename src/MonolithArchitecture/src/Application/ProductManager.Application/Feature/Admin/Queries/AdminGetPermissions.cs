namespace ProductManager.Application.Feature.Admin.Queries;

public record AdminGetPermissionsQuery : IQuery<ApiResponse>;
internal class AdminGetPermissionsHandler : IQueryHandler<AdminGetPermissionsQuery, ApiResponse>
{
    private readonly IAdminRepository _adminRepository;
    public AdminGetPermissionsHandler(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }
    public async Task<ApiResponse> HandleAsync(AdminGetPermissionsQuery query, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return _adminRepository.AdminGetPermissions();
    }
}
