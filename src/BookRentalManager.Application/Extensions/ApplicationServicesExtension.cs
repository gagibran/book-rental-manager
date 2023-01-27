using BookRentalManager.Application.BooksAuthors.QueryHandlers;
using BookRentalManager.Application.Books.QueryHandlers;
using BookRentalManager.Application.Customers.CommandHandlers;
using BookRentalManager.Application.Customers.QueryHandlers;
using Microsoft.Extensions.DependencyInjection;
using BookRentalManager.Application.Mappers;
using BookRentalManager.Application.Common;
using BookRentalManager.Application.Books.CommandHandlers;

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
        serviceCollection.AddScoped<ICommandHandler<CreateCustomerCommand, CustomerCreatedDto>, CreateCustomerCommandHandler>();
        serviceCollection.AddScoped<ICommandHandler<CreateBookCommand, BookCreatedDto>, CreateBookCommandHandler>();
        return serviceCollection;
    }

    private static IServiceCollection AddApplicationQueriesService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IQueryHandler<GetCustomersBySearchParameterQuery, IReadOnlyList<GetCustomerDto>>, GetCustomersBySearchParameterQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetCustomerByIdQuery, GetCustomerDto>, GetCustomerByIdQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetBookAuthorsBySearchParameterQuery, IReadOnlyList<GetBookAuthorDto>>, GetBookAuthorsBySearchParameterQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetBooksBySearchParameterQuery, IReadOnlyList<GetBookDto>>, GetBooksBySearchParameterQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetBookByIdQuery, GetBookDto>, GetBookByIdQueryHandler>();
        return serviceCollection;
    }

    private static IServiceCollection AddApplicationMappersServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IMapper<Customer, GetCustomerDto>, GetCustomerDtoMapper>();
        serviceCollection.AddTransient<IMapper<BookAuthor, GetBookAuthorDto>, GetBookAuthorDtoMapper>();
        serviceCollection.AddTransient<IMapper<IReadOnlyList<Book>, IReadOnlyList<GetCustomerBookDto>>, GetCustomerBookDtosMapper>();
        serviceCollection.AddTransient<IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookAuthorBookDto>>, GetBookAuthorBookDtosMapper>();
        serviceCollection.AddTransient<IMapper<Book, GetBookDto>, GetBookDtoMapper>();
        serviceCollection.AddTransient<IMapper<Book, BookCreatedDto>, BookCreatedDtoMapper>();
        serviceCollection.AddTransient<IMapper<Customer?, GetRentedByDto>, GetRentedByDtoMapper>();
        serviceCollection.AddTransient<IMapper<IReadOnlyList<BookAuthor>, IReadOnlyList<GetBookBookAuthorDto>>, GetBookBookAuthorDtosMapper>();
        serviceCollection.AddTransient<IMapper<Customer, CustomerCreatedDto>, CustomerCreatedDtoMapper>();
        return serviceCollection;
    }
}
