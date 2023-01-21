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
        serviceCollection.AddScoped<IQueryHandler<GetBookAuthorsWithBooksAndSearchParamQuery, IReadOnlyList<GetBookAuthorDto>>, GetBookAuthorsWithBooksAndSearchParamQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetCustomerWithBooksByIdQuery, GetCustomerDto>, GetCustomerWithBooksByIdQueryHandler>();
        return serviceCollection;
    }

    private static IServiceCollection AddApplicationMappersServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IMapper<Customer, GetCustomerDto>, GetCustomerDtoMapper>();
        serviceCollection.AddTransient<IMapper<BookAuthor, GetBookAuthorDto>, GetBookAuthorDtoMapper>();
        serviceCollection.AddTransient<IMapper<IReadOnlyList<Book>, IReadOnlyList<GetCustomerBookDto>>, GetCustomerBookDtosMapper>();
        serviceCollection.AddTransient<IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookAuthorBookDto>>, GetBookAuthorBookDtosMapper>();
        serviceCollection.AddTransient<IMapper<Book, GetBookDto>, GetBookDtoMapper>();
        serviceCollection.AddTransient<IMapper<Customer?, GetRentedByDto>, GetRentedByDtoMapper>();
        serviceCollection.AddTransient<IMapper<IReadOnlyList<BookAuthor>, IReadOnlyList<GetBookBookAuthorDto>>, GetBookBookAuthorDtosMapper>();
        return serviceCollection;
    }
}
