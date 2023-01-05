using BookRentalManager.Infrastructure.Common;
using BookRentalManager.Infrastructure.Data.Seeds;
using Microsoft.Extensions.DependencyInjection;

namespace BookRentalManager.Infrastructure.Extensions;

public static class InfrastructureServicesExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddTransient<IRepository<Customer>, Repository<Customer>>();
        services.AddTransient<IRepository<Book>, Repository<Book>>();
        services.AddTransient<IRepository<BookAuthor>, Repository<BookAuthor>>();
        services.AddScoped<TestDataSeeder>();
        return services;
    }
}
