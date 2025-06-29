using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ProductManager.Shared.Permission;

public class EntityPermissions
{
    private static readonly ReadOnlyCollection<EntityPermission> _allPermissions;
    /// <summary>
    ///     Static constructor that generates application permissions based on constant values defined in nested classes.
    /// </summary>
    static EntityPermissions()
    {
        var allPermissions = new List<EntityPermission>();
        IEnumerable<object> permissionClasses =
            typeof(Permissions).GetNestedTypes(BindingFlags.Static | BindingFlags.Public).Cast<TypeInfo>();

        foreach (TypeInfo permissionClass in permissionClasses)
        {
            var permissions = permissionClass.DeclaredFields.Where(f => f.IsLiteral);
            foreach (var permission in permissions)
            {
                var applicationPermission = new EntityPermission
                {
                    Value = permission.GetValue(null)!.ToString()!,
                    Name = permission.GetValue(null)!.ToString()!.Replace('.', ' '),
                    GroupName = permissionClass.Name
                };

                var attributes = (DisplayAttribute[])permission.GetCustomAttributes(typeof(DisplayAttribute), false);
                applicationPermission.Description = attributes.Length > 0 ? attributes[0].Description : applicationPermission.Name;

                allPermissions.Add(applicationPermission);
            }
        }

        _allPermissions = allPermissions.AsReadOnly();
    }

    /// <summary>
    ///     Gets all permissions available.
    /// </summary>
    public IEnumerable<EntityPermission> GetAllPermissions() => _allPermissions;

    public EntityPermission GetPermissionByName(string permissionName)
        => _allPermissions.FirstOrDefault(p => p.Name == permissionName)!;

    public EntityPermission GetPermissionByValue(string permissionValue)
        => _allPermissions.FirstOrDefault(p => p.Value == permissionValue)!;

    public string[] GetAllPermissionValues()
        => _allPermissions.OrderBy(p => p.Value).Select(p => p.Value).ToArray();

    public string[] GetAllPermissionNames()
        => _allPermissions.OrderBy(p => p.Name).Select(p => p.Name).ToArray();
}
