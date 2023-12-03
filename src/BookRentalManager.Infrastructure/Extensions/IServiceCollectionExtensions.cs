using BookRentalManager.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;

namespace BookRentalManager.Infrastructure.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IRepository<Author>, Repository<Author>>();
        serviceCollection.AddScoped<IRepository<Book>, Repository<Book>>();
        serviceCollection.AddScoped<IRepository<Customer>, Repository<Customer>>();
        return serviceCollection;
    }
}
