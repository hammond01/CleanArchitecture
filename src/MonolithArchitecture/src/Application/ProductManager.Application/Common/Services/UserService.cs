using ProductManager.Domain.Repositories;

namespace ProductManager.Application.Common.Services;

public class UserService
{
    private readonly IAdminRepository _adminRepository;
    public UserService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }
    public string GetUserLogin() => _adminRepository.GetUserLogin();
}
