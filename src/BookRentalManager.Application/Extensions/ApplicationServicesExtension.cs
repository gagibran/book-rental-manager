using BookRentalManager.Application.BooksAuthors.QueryHandlers;
using BookRentalManager.Application.Books.Queries;
using BookRentalManager.Application.Books.QueryHandlers;
using BookRentalManager.Application.Customers.CommandHandlers;
using BookRentalManager.Application.Customers.QueryHandlers;
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
        serviceCollection.AddScoped<IQueryHandler<GetCustomersWithSearchParamQuery, IReadOnlyList<GetCustomerDto>>, GetCustomersWithSearchParamQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetBookAuthorsWithSearchParamQuery, IReadOnlyList<GetBookAuthorDto>>, GetBookAuthorsWithSearchParamQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetCustomerWithBooksByIdQuery, GetCustomerDto>, GetCustomerWithBooksByIdQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetBooksWithSearchParamQuery, IReadOnlyList<GetBookDto>>, GetBooksWithSearchParamQueryHandler>();
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
