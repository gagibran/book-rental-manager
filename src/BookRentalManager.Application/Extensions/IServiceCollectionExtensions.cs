using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BookRentalManager.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IDispatcher, Dispatcher>();
        serviceCollection.AddServicesFromAssembly(
            typeof(ICommand).Assembly,
            ServiceLifetime.Scoped,
            typeof(ICommandHandler<>),
            typeof(ICommandHandler<,>),
            typeof(IQueryHandler<,>));
        serviceCollection.AddServicesFromAssembly(
            typeof(IMapper<,>).Assembly,
            ServiceLifetime.Transient,
            typeof(IMapper<,>));
        return serviceCollection;
    }

    private static void AddServicesFromAssembly(
        this IServiceCollection serviceCollection,
        Assembly assemblyToScan,
        ServiceLifetime serviceLifetime,
        params Type[] genericInterfacesToRegister)
    {
        IEnumerable<Type> handlerImplementations = assemblyToScan
            .GetTypes()
            .Where(type => type.IsConcrete() && type.HasGenericInterfaces(genericInterfacesToRegister));
        foreach (Type handlerImplementation in handlerImplementations)
        {
            Type handlerInterface = handlerImplementation.GetInterfaces().First();
            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    serviceCollection.AddSingleton(handlerInterface, handlerImplementation);
                    break;
                case ServiceLifetime.Scoped:
                    serviceCollection.AddScoped(handlerInterface, handlerImplementation);
                    break;
                case ServiceLifetime.Transient:
                    serviceCollection.AddTransient(handlerInterface, handlerImplementation);
                    break;
                default:
                    break;
            }
        }
    }
}
