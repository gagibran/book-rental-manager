using BookRentalManager.Application.Authors.QueryHandlers;
using BookRentalManager.Application.Books.QueryHandlers;
using BookRentalManager.Application.Customers.CommandHandlers;
using BookRentalManager.Application.Customers.QueryHandlers;
using Microsoft.Extensions.DependencyInjection;
using BookRentalManager.Application.DtoMappers;
using BookRentalManager.Application.Books.CommandHandlers;
using BookRentalManager.Application.SortParametersMappers;
using BookRentalManager.Application.Authors.CommandHandlers;

namespace BookRentalManager.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IDispatcher, Dispatcher>();
        serviceCollection.AddCommandServices();
        serviceCollection.AddQueryServices();
        serviceCollection.AddDtoMapperServices();
        serviceCollection.AddSortParametersMapperServices();
        return serviceCollection;
    }

    private static IServiceCollection AddCommandServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICommandHandler<CreateCustomerCommand, CustomerCreatedDto>, CreateCustomerCommandHandler>();
        serviceCollection.AddScoped<ICommandHandler<CreateBookForAuthorCommand, BookCreatedForAuthorDto>, CreateBookForAuthorCommandHandler>();
        serviceCollection.AddScoped<ICommandHandler<CreateAuthorCommand, AuthorCreatedDto>, CreateAuthorCommandHandler>();
        return serviceCollection;
    }

    private static IServiceCollection AddQueryServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IQueryHandler<GetCustomersByQueryParametersQuery, PaginatedList<GetCustomerDto>>, GetCustomersByQueryParametersQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetCustomerByIdQuery, GetCustomerDto>, GetCustomerByIdQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetAuthorsByQueryParametersQuery, PaginatedList<GetAuthorDto>>, GetAuthorsByQueryParametersQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetBooksByQueryParametersFromAuthorQuery, PaginatedList<GetBookDto>>, GetBooksByQueryParametersFromAuthorQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetBookByIdFromAuthorQuery, GetBookDto>, GetBookByIdFromAuthorQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetAuthorByIdQuery, GetAuthorDto>, GetAuthorByIdQueryHandler>();
        return serviceCollection;
    }

    private static IServiceCollection AddDtoMapperServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IMapper<Customer, GetCustomerDto>, CustomerToGetCustomerDtoMapper>();
        serviceCollection.AddTransient<IMapper<Author, GetAuthorDto>, AuthorToGetAuthorDtoMapper>();
        serviceCollection.AddTransient<IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookRentedByCustomerDto>>, BooksToGetBookRentedByCustomerDtosMapper>();
        serviceCollection.AddTransient<IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookFromAuthorDto>>, BooksToGetBookFromAuthorDtosMapper>();
        serviceCollection.AddTransient<IMapper<Book, GetBookDto>, BookToGetBookDtoMapper>();
        serviceCollection.AddTransient<IMapper<Book, BookCreatedForAuthorDto>, BookToBookCreatedForAuthorDtoMapper>();
        serviceCollection.AddTransient<IMapper<Customer?, GetCustomerThatRentedBookDto>, CustomerToGetCustomerThatRentedBookDtoMapper>();
        serviceCollection.AddTransient<IMapper<IReadOnlyList<Author>, IReadOnlyList<GetAuthorFromBookDto>>, AuthorsToGetAuthorFromBookDtosMapper>();
        serviceCollection.AddTransient<IMapper<Customer, CustomerCreatedDto>, CustomerToCustomerCreatedDtoMapper>();
        serviceCollection.AddTransient<IMapper<Author, AuthorCreatedDto>, AuthorToAuthorCreatedDtoMapper>();
        return serviceCollection;
    }

    private static IServiceCollection AddSortParametersMapperServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IMapper<AuthorSortParameters, Result<string>>, AuthorSortParametersMapper>();
        serviceCollection.AddTransient<IMapper<BookSortParameters, Result<string>>, BookSortParametersMapper>();
        serviceCollection.AddTransient<IMapper<CustomerSortParameters, Result<string>>, CustomerSortParametersMapper>();
        return serviceCollection;
    }
}
