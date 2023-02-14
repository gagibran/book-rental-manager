using BookRentalManager.Application.Authors.QueryHandlers;
using BookRentalManager.Application.Books.QueryHandlers;
using BookRentalManager.Application.Customers.CommandHandlers;
using BookRentalManager.Application.Customers.QueryHandlers;
using Microsoft.Extensions.DependencyInjection;
using BookRentalManager.Application.Mappers;
using BookRentalManager.Application.Books.CommandHandlers;

namespace BookRentalManager.Application.Extensions;

public static class IServiceCollectionApplicationExtensions
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
        serviceCollection.AddScoped<ICommandHandler<CreateCustomerCommand, CustomerCreatedDto>, CreateCustomerCommandHandler>();
        serviceCollection.AddScoped<ICommandHandler<CreateBookForAuthorCommand, BookCreatedForAuthorDto>, CreateBookForAuthorCommandHandler>();
        return serviceCollection;
    }

    private static IServiceCollection AddApplicationQueriesService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IQueryHandler<GetCustomersByQueryParametersQuery, PaginatedList<GetCustomerDto>>, GetCustomersByQueryParametersQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetCustomerByIdQuery, GetCustomerDto>, GetCustomerByIdQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetAuthorsByQueryParametersQuery, PaginatedList<GetAuthorDto>>, GetAuthorsByQueryParametersQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetBooksByQueryParametersFromAuthorQuery, PaginatedList<GetBookDto>>, GetBooksByQueryParametersFromAuthorQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetBookByIdFromAuthorQuery, GetBookDto>, GetBookByIdFromAuthorQueryHandler>();
        return serviceCollection;
    }

    private static IServiceCollection AddApplicationMappersServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IMapper<Customer, GetCustomerDto>, GetCustomerDtoMapper>();
        serviceCollection.AddTransient<IMapper<Author, GetAuthorDto>, GetAuthorDtoMapper>();
        serviceCollection.AddTransient<IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookRentedByCustomerDto>>, GetBookRentedByCustomerDtosMapper>();
        serviceCollection.AddTransient<IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookFromAuthorDto>>, GetBookFromAuthorDtosMapper>();
        serviceCollection.AddTransient<IMapper<Book, GetBookDto>, GetBookDtoMapper>();
        serviceCollection.AddTransient<IMapper<Book, BookCreatedForAuthorDto>, BookCreatedForAuthorDtoMapper>();
        serviceCollection.AddTransient<IMapper<Customer?, GetCustomerThatRentedBookDto>, GetCustomerThatRentedBookDtoMapper>();
        serviceCollection.AddTransient<IMapper<IReadOnlyList<Author>, IReadOnlyList<GetAuthorFromBookDto>>, GetAuthorFromBookDtosMapper>();
        serviceCollection.AddTransient<IMapper<Customer, CustomerCreatedDto>, CustomerCreatedDtoMapper>();
        return serviceCollection;
    }
}
