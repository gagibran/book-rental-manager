using BookRentalManager.Infrastructure.Common;
using BookRentalManager.Infrastructure.Data.Seeds;
using Microsoft.Extensions.DependencyInjection;

namespace BookRentalManager.Infrastructure.Extensions;

public static class IServiceCollectionInfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<TestDataSeeder>();
        return services;
    }
}
