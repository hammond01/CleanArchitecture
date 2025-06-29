namespace ProductManager.Persistence.Extensions;

public static class UlidExtension
{
    public static bool IsNullOrEmpty(this Ulid? ulid) => ulid == null || ulid == Ulid.Empty;

    public static string Generate() => Ulid.NewUlid().ToString();
}
