namespace ProductManager.Domain.Identity;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    string UserId { get; }

    string UserName { get; }
}
