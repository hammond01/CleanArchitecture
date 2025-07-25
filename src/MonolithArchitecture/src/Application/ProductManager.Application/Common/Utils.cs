using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Queries;

namespace ProductManager.Application.Common;

internal static class Utils
{
    public static bool IsHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var typeDefinition = type.GetGenericTypeDefinition();

        return typeDefinition == typeof(ICommandHandler<,>)
               || typeDefinition == typeof(IQueryHandler<,>);
    }
}
