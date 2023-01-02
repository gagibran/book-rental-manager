using BookRentalManager.Application.Common;
using BookRentalManager.Application.CustomerCqrs.CommandHandlers;
using BookRentalManager.Application.CustomerCqrs.Queries;
using BookRentalManager.Application.CustomerCqrs.QueryHandlers;
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
        services.AddTransient<IQueryHandler<GetAllCustomersQuery, IReadOnlyList<Customer>>, GetAllCustomersQueryHandler>();
        return services;
    }
}
