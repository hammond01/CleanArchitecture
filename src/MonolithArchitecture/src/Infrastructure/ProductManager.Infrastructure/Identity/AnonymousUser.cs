using ProductManager.Domain.Identity;
namespace ProductManager.Infrastructure.Identity;

public class AnonymousUser : ICurrentUser
{
    public bool IsAuthenticated => false;

    public string UserId => string.Empty;

    public string UserName => string.Empty;
}
