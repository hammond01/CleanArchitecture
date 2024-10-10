using System.Text;
namespace ProductManager.CrossCuttingConcerns.ExtensionMethods;

public static class TypeExtensions
{
    public static bool HasInterface(this Type type, Type interfaceType) => type.GetInterfacesOf(interfaceType).Length > 0;

    private static Type[] GetInterfacesOf(this Type type, Type interfaceType)
        => type.FindInterfaces(filter: (i, _) => i.GetGenericTypeDefinitionSafe() == interfaceType, null);

    private static Type GetGenericTypeDefinitionSafe(this Type type) => type.IsGenericType
        ? type.GetGenericTypeDefinition()
        : type;

    public static Type MakeGenericTypeSafe(this Type type, params Type[] typeArguments)
        => type.IsGenericType && !type.GenericTypeArguments.Any()
            ? type.MakeGenericType(typeArguments)
            : type;

    public static string GenerateMappingCode(this Type type)
    {
        var names = type.GetProperties().Select(x => x.Name);

        var text1 = new StringBuilder();
        var text2 = new StringBuilder();
        var text3 = new StringBuilder();
        var text4 = new StringBuilder();

        foreach (var name in names)
        {
            text1.Append($"a.{name} = {name};{Environment.NewLine}");
            text2.Append($"{name} = b.{name};{Environment.NewLine}");
            text3.Append($"{name} = b.{name},{Environment.NewLine}");
            text4.Append($"a.{name} = b.{name};{Environment.NewLine}");
        }

        return text1
               + "--------------------------------------" + Environment.NewLine
               + text2
               + "--------------------------------------" + Environment.NewLine
               + text3
               + "--------------------------------------" + Environment.NewLine
               + text4;
    }
}
