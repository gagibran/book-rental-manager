namespace BookRentalManager.Domain.Extensions;

public static class IQueryableExtensions
{
    // More information: https://github.com/dotnet/efcore/blob/main/src/EFCore/Extensions/EntityFrameworkQueryableExtensions.cs.
    public static async Task<List<TItem>> ToListAsync<TItem>(this IQueryable<TItem> query, CancellationToken cancellationToken = default)
    {
        var items = new List<TItem>();
        await foreach (TItem item in query.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            items.Add(item);
        }
        return items;
    }

    // More information: https://github.com/dotnet/efcore/blob/main/src/EFCore/Extensions/EntityFrameworkQueryableExtensions.cs.
    public static IAsyncEnumerable<TItem> AsAsyncEnumerable<TItem>(this IQueryable<TItem> query)
    {
        if (query is IAsyncEnumerable<TItem> asyncEnumerable)
        {
            return asyncEnumerable;
        }
        throw new QueryableIsNotAsyncEnumerableException();
    }

    public static IQueryable<TItem> OrderByPropertyName<TItem>(this IQueryable<TItem> query, string propertyNamesSeparatedByComma)
    {
        Type tItem = typeof(TItem);
        foreach (string propertyName in propertyNamesSeparatedByComma.Split(','))
        {
            var isQueryOrdered = query.Expression.Type == typeof(IOrderedQueryable<TItem>);
            var formattedPropertyName = propertyName;
            bool isDescending = propertyName.EndsWith("Desc", StringComparison.OrdinalIgnoreCase);
            if (isDescending)
            {
                formattedPropertyName = propertyName.Remove(propertyName.Length - 4);
            }
            ParameterExpression parameter = Expression.Parameter(tItem, "entity");
            Expression property = formattedPropertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
            property = Expression.Convert(property, typeof(object));
            Expression<Func<TItem, object?>> body = Expression.Lambda<Func<TItem, object?>>(property, parameter);
            if (isDescending && !isQueryOrdered)
            {
                query = query.OrderByDescending(body);
            }
            else if (!isDescending && !isQueryOrdered)
            {
                query = query.OrderBy(body);
            }
            else if (isDescending && isQueryOrdered)
            {
                query = ((IOrderedQueryable<TItem>)query).ThenByDescending(body);
            }
            else if (!isDescending && isQueryOrdered)
            {
                query = ((IOrderedQueryable<TItem>)query).ThenBy(body);
            }
        }
        return query;
    }
}
