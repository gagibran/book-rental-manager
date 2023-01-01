using BookRentalManager.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;

namespace BookRentalManager.Infrastructure.Extensions;

public static class InfrastructureServicesExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddTransient<IRepository<Customer>, Repository<Customer>>();
        services.AddTransient<IRepository<Book>, Repository<Book>>();
        services.AddTransient<IRepository<BookAuthor>, Repository<BookAuthor>>();
        return services;
    }
}
