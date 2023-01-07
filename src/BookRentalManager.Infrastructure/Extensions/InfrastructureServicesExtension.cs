using BookRentalManager.Infrastructure.Common;
using BookRentalManager.Infrastructure.Data.Seeds;
using Microsoft.Extensions.DependencyInjection;

namespace BookRentalManager.Infrastructure.Extensions;

public static class InfrastructureServicesExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Customer>, Repository<Customer>>();
        services.AddScoped<IRepository<Book>, Repository<Book>>();
        services.AddScoped<IRepository<BookAuthor>, Repository<BookAuthor>>();
        services.AddScoped<TestDataSeeder>();
        return services;
    }
}
