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
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddApplicationQueriesAndCommandsServices();
        services.AddApplicationMappersServices();
        return services;
    }

    private static IServiceCollection AddApplicationQueriesAndCommandsServices(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<AddNewCustomerCommand>, AddNewCustomerCommandHandler>();
        services.AddScoped<IQueryHandler<GetCustomersQuery, IReadOnlyList<GetCustomerDto>>, GetCustomersQueryHandler>();
        services.AddScoped<IQueryHandler<GetCustomerByIdQuery, GetCustomerDto>, GetCustomerByIdQueryHandler>();
        return services;
    }

    private static IServiceCollection AddApplicationMappersServices(this IServiceCollection services)
    {
        services.AddTransient<IMapper<Customer, GetCustomerDto>, GetCustomerDtoMapper>();
        services.AddTransient<IMapper<IReadOnlyList<Book>, IReadOnlyList<GetCustomerBooksDto>>, GetCustomerBooksDtoMapper>();
        services.AddTransient<IMapper<IReadOnlyList<BookAuthor>, IReadOnlyList<GetBookAuthorsForCustomerBooksDto>>, GetBookAuthorsForCustomerBooksDtoMapper>();
        return services;
    }
}
