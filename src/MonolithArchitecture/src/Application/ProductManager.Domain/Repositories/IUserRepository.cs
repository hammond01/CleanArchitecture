using ProductManager.Domain.Entities.Identity;
namespace ProductManager.Domain.Repositories;

public class UserQueryOptions
{
    public bool IncludePasswordHistories { get; set; }
    public bool IncludeClaims { get; set; }
    public bool IncludeUserRoles { get; set; }
    public bool IncludeRoles { get; set; }
    public bool IncludeTokens { get; set; }
    public bool AsNoTracking { get; set; }
}
public interface IUserRepository
{
    IQueryable<User> Get(UserQueryOptions queryOptions);
}
