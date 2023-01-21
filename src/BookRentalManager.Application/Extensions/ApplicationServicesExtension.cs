using BookRentalManager.Application.BookAuthorCqrs.QueryHandlers;
using BookRentalManager.Application.CustomerCqrs.CommandHandlers;
using BookRentalManager.Application.CustomerCqrs.QueryHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace BookRentalManager.Application.Extensions;

public static class ApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IDispatcher, Dispatcher>();
        serviceCollection.AddApplicationCommandsServices();
        serviceCollection.AddApplicationQueriesService();
        serviceCollection.AddApplicationMappersServices();
        return serviceCollection;
    }

    private static IServiceCollection AddApplicationCommandsServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICommandHandler<AddNewCustomerCommand>, AddNewCustomerCommandHandler>();
        return serviceCollection;
    }

    private static IServiceCollection AddApplicationQueriesService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IQueryHandler<GetCustomersWithBooksAndSearchParamQuery, IReadOnlyList<GetCustomerDto>>, GetCustomersWithBooksAndSearchParamQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetBookAuthorsQuery, IReadOnlyList<GetBookAuthorDto>>, GetBookAuthorsQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetCustomerByIdQuery, GetCustomerDto>, GetCustomerByIdQueryHandler>();
        return serviceCollection;
    }

    private static IServiceCollection AddApplicationMappersServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IMapper<Customer, GetCustomerDto>, GetCustomerDtoMapper>();
        serviceCollection.AddTransient<IMapper<BookAuthor, GetBookAuthorDto>, GetBookAuthorDtoMapper>();
        serviceCollection.AddTransient<IMapper<IReadOnlyList<Book>, IReadOnlyList<GetCustomerBookDto>>, GetCustomerBooksDtoMapper>();
        serviceCollection.AddTransient<IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookAuthorBookDto>>, GetBookAuthorBooksDtoMapper>();
        return serviceCollection;
    }
}
