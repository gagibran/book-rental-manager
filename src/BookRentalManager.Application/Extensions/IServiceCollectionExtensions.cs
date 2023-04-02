using System.Reflection;
using BookRentalManager.Application.Decorators;
using Microsoft.Extensions.DependencyInjection;

namespace BookRentalManager.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IDispatcher, Dispatcher>();
        serviceCollection.AddServicesFromAssembly(typeof(IMapper<,>), ServiceLifetime.Transient);
        serviceCollection.AddServicesFromAssembly(
            typeof(IRequestHandler<>),
            ServiceLifetime.Scoped,
            typeof(ExecutionTimeLoggingDecorator<>),
            typeof(HandlerLoggingDecorator<>));
        serviceCollection.AddServicesFromAssembly(
            typeof(IRequestHandler<,>),
            ServiceLifetime.Scoped,
            typeof(ExecutionTimeLoggingDecorator<,>),
            typeof(HandlerLoggingDecorator<,>));
        return serviceCollection;
    }

    public static void AddServicesFromAssembly(
        this IServiceCollection serviceCollection,
        Type genericInterfaceToRegister,
        ServiceLifetime serviceLifetime)
    {
        IEnumerable<Type> typeImplementations = GetTypeImplementations(genericInterfaceToRegister);
        foreach (Type typeImplementation in typeImplementations)
        {
            AddServices(serviceCollection, typeImplementation, serviceLifetime);
        }
    }

    public static void AddServicesFromAssembly(
        this IServiceCollection serviceCollection,
        Type genericInterfaceToRegister,
        ServiceLifetime serviceLifetime,
        params Type[] decorators)
    {
        IEnumerable<Type> typeImplementations = GetTypeImplementations(genericInterfaceToRegister);
        foreach (Type typeImplementation in typeImplementations)
        {
            AddDecoratedServices(serviceCollection, typeImplementation, serviceLifetime, decorators);
        }
    }

    private static void AddDecoratedServices(
        this IServiceCollection serviceCollection,
        Type typeImplementation,
        ServiceLifetime serviceLifetime,
        Type[] decorators)
    {
        Type typeInterface = typeImplementation.GetInterfaces().First();
        Func<IServiceProvider, object> factory = DecorateBehaviors(typeImplementation, typeInterface, decorators);
        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                serviceCollection.AddSingleton(typeInterface, factory);
                break;
            case ServiceLifetime.Scoped:
                serviceCollection.AddScoped(typeInterface, factory);
                break;
            case ServiceLifetime.Transient:
                serviceCollection.AddTransient(typeInterface, factory);
                break;
            default:
                break;
        }
    }

    private static void AddServices(
        this IServiceCollection serviceCollection,
        Type typeImplementation,
        ServiceLifetime serviceLifetime)
    {
        Type typeInterface = typeImplementation.GetInterfaces().First();
        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                serviceCollection.AddSingleton(typeInterface, typeImplementation);
                break;
            case ServiceLifetime.Scoped:
                serviceCollection.AddScoped(typeInterface, typeImplementation);
                break;
            case ServiceLifetime.Transient:
                serviceCollection.AddTransient(typeInterface, typeImplementation);
                break;
            default:
                break;
        }
    }

    private static IEnumerable<Type> GetTypeImplementations(Type genericInterfaceToRegister)
    {
        return genericInterfaceToRegister.Assembly
            .GetTypes()
            .Where(type => type.IsConcrete() && type.HasGenericInterfaces(genericInterfaceToRegister));
    }

    private static Func<IServiceProvider, object> DecorateBehaviors(
        Type typeImplementation,
        Type typeInterface,
        params Type[] decorators)
    {
        return (serviceProvider) =>
        {
            IEnumerable<Type> implementationTypeAndDecorators = decorators
                .Concat(new Type[] { typeImplementation })
                .Reverse();
            IEnumerable<ConstructorInfo> ctors = implementationTypeAndDecorators.Select(type =>
            {
                Type implementation = type.IsGenericType ? type.MakeGenericType(typeInterface.GenericTypeArguments) : type;
                return implementation.GetConstructors().First();
            });
            return InvokeConstructors(ctors, typeImplementation, serviceProvider);
        };
    }

    private static object InvokeConstructors(
        IEnumerable<ConstructorInfo> constructors,
        Type typeImplementation,
        IServiceProvider serviceProvider)
    {
        object? currentConstructorInvocation = null;
        foreach (ConstructorInfo constructor in constructors)
        {
            ParameterInfo[] constructorParameters = constructor.GetParameters();
            List<object?> constructorArguments = GetConstructorParameters(
                constructorParameters,
                typeImplementation,
                currentConstructorInvocation,
                serviceProvider);
            currentConstructorInvocation = constructor.Invoke(constructorArguments.ToArray());
        }
        return currentConstructorInvocation ?? typeImplementation;
    }

    private static List<object?> GetConstructorParameters(
        IEnumerable<ParameterInfo> constructorParameters,
        Type typeImplementation,
        object? currentConstructorInvocation,
        IServiceProvider serviceProvider)
    {
        var constructorArguments = new List<object?>();
        Type typeInterface = typeImplementation.GetInterfaces().First();
        foreach (ParameterInfo constructorParameter in constructorParameters)
        {
            Type constructorParameterType = constructorParameter.ParameterType;
            if (constructorParameterType.GetGenericTypeDefinition() == typeInterface.GetGenericTypeDefinition())
            {
                constructorArguments.Add(currentConstructorInvocation);
                continue;
            }
            object? service = serviceProvider.GetService(constructorParameterType);
            if (service is not null)
            {
                constructorArguments.Add(service);
            }
        }
        return constructorArguments;
    }
}
