using System.ComponentModel.DataAnnotations;

namespace ProductManager.Shared.Permission;

public static class Permissions
{
    #region Admin
    public static class User
    {
        [Display(Name = "CreateUserPermission")]
        public const string Create = "User.Create";

        [Display(Name = "UpdateUserPermission")]
        public const string Update = "User.Update";

        [Display(Name = "ReadUserPermission")]
        public const string Read = "User.Read";

        [Display(Name = "DeleteUserPermission")]
        public const string Delete = "User.Delete";
    }

    public static class Role
    {
        [Display(Name = "CreateRolePermission")]
        public const string Create = "Role.Create";

        [Display(Name = "UpdateRolePermission")]
        public const string Update = "Role.Update";

        [Display(Name = "ReadRolePermission")]
        public const string Read = "Role.Read";

        [Display(Name = "DeleteRolePermission")]
        public const string Delete = "Role.Delete";
    }
    #endregion
}
