using BookRentalManager.Infrastructure.Common;
using BookRentalManager.Infrastructure.Data;
using BookRentalManager.Infrastructure.Decorators;
using Microsoft.Extensions.DependencyInjection;

namespace BookRentalManager.Infrastructure.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped(typeof(IRepository<Author>), serviceProvider =>
        {
            var bookRentalManagerDbContext = serviceProvider.GetRequiredService<BookRentalManagerDbContext>();
            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
            var authorRepository = new Repository<Author>(bookRentalManagerDbContext);
            return new CachedAuthorRepository(authorRepository, memoryCache);
        });
        serviceCollection.AddScoped<IRepository<Book>, Repository<Book>>();
        serviceCollection.AddScoped<IRepository<Customer>, Repository<Customer>>();
        return serviceCollection;
    }
}
