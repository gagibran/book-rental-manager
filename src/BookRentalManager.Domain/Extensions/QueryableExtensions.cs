// I copied this code directly from the EntityFrameworkQueryableExtensions.cs file, from Entity Framework Core.
// I did this so that I wouldn't need to install EF Core in the Domain project, therefore coupling it to this ORM.
// More information: https://github.com/dotnet/efcore/blob/main/src/EFCore/Extensions/EntityFrameworkQueryableExtensions.cs.

namespace BookRentalManager.Domain.Extensions;

public static class QueryableExtensions
{
    public static async Task<List<TItem>> ToListAsync<TItem>(this IQueryable<TItem> query, CancellationToken cancellationToken = default)
    {
        var items = new List<TItem>();
        await foreach (TItem item in query.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            items.Add(item);
        }
        return items;
    }

    public static IAsyncEnumerable<TItem> AsAsyncEnumerable<TItem>(this IQueryable<TItem> query)
    {
        if (query is IAsyncEnumerable<TItem> asyncEnumerable)
        {
            return asyncEnumerable;
        }
        throw new QueryableIsNotAsyncEnumerableException();
    }
}
