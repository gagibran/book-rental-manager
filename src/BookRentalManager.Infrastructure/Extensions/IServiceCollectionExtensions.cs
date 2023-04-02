using BookRentalManager.Infrastructure.Common;
using BookRentalManager.Infrastructure.Data.Seeds;
using Microsoft.Extensions.DependencyInjection;

namespace BookRentalManager.Infrastructure.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        serviceCollection.AddScoped<TestDataSeeder>();
        return serviceCollection;
    }
}
