namespace BookRentalManager.Application.Extensions;

public static class TypeExtensions
{
    public static bool IsConcrete(this Type type)
    {
        return !type.IsAbstract && !type.IsInterface;
    }
    
    public static bool HasGenericInterfaces(this Type type, params Type[] genericInterfaceTypesToFilter)
    {
        return type
            .GetInterfaces()
            .Any(interfaceType =>
            {
                return interfaceType.IsGenericType && genericInterfaceTypesToFilter.Contains(interfaceType.GetGenericTypeDefinition());
            });
    }
}
