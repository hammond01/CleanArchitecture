using ProductManager.Domain.Identity;
using ProductManager.Domain.Repositories;

namespace ProductManager.Application.Common.Services;

public class UserService
{
    private readonly ICurrentUser _currentUser;

    public UserService(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public string GetUserLogin() => _currentUser.UserName ?? string.Empty;

    public string GetUserId() => _currentUser.UserId;

    public bool IsAuthenticated() => _currentUser.IsAuthenticated;
}
