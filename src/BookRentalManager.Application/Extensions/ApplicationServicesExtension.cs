using BookRentalManager.Application.Common;
using BookRentalManager.Application.CustomerCqrs.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace BookRentalManager.Application.Extensions;

public static class ApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IDispatcher, Dispatcher>();
        services.AddApplicationCustomerServices();
        return services;
    }

    private static IServiceCollection AddApplicationCustomerServices(this IServiceCollection services)
    {
        services.AddTransient<ICommandHandler<AddNewCustomerCommand>, AddNewCustomerCommandHandler>();
        return services;
    }
}
