namespace ProductManager.Shared.Permission;

public class EntityPermission
{
    public EntityPermission()
    {
    }

    public EntityPermission(string name, string value, string groupName, string? description = null)
    {
        Name = name;
        Value = value;
        GroupName = groupName;
        Description = description;
    }

    public string Name { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string GroupName { get; set; } = default!;
    public string? Description { get; set; }

    public override string ToString() => Value;

    public static implicit operator string(EntityPermission permission) => permission.Value;
}
