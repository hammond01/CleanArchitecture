using System.Reflection;
using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Queries;
using ProductManager.Shared.ExtensionMethods;

namespace ProductManager.Application.Decorators;

internal static class Mappings
{

    public static readonly Dictionary<Type, Type> AttributeToCommandHandler = new Dictionary<Type, Type>();

    public static readonly Dictionary<Type, Type> AttributeToQueryHandler = new Dictionary<Type, Type>();
    static Mappings()
    {
        var decorators = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in decorators)
        {
            if (type.HasInterface(typeof(ICommandHandler<,>)))
            {
                var decoratorAttribute =
                    (MappingAttribute)type.GetCustomAttributes(false).FirstOrDefault(x => x is MappingAttribute)!;

                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (decoratorAttribute != null)
                {
                    AttributeToCommandHandler[decoratorAttribute.Type!] = type;
                }
            }
            else if (type.HasInterface(typeof(IQueryHandler<,>)))
            {
                var decoratorAttribute =
                    (MappingAttribute)type.GetCustomAttributes(false).FirstOrDefault(x => x is MappingAttribute)!;

                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (decoratorAttribute != null)
                {
                    AttributeToQueryHandler[decoratorAttribute.Type!] = type;
                }
            }
        }
    }
}
[AttributeUsage(AttributeTargets.Class)]
public sealed class MappingAttribute : Attribute
{
    public Type? Type { get; set; }
}
