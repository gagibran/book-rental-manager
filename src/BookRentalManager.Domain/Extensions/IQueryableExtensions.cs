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
        if (string.IsNullOrWhiteSpace(propertyNamesSeparatedByComma))
        {
            return query;
        }
        foreach (string propertyName in propertyNamesSeparatedByComma.Split(','))
        {
            bool isDescending = propertyName.EndsWith("Desc");
            var isQueryOrdered = query.Expression.Type == typeof(IOrderedQueryable<TItem>);
            var formattedPropertyName = isDescending ? propertyName[..^4] : propertyName;
            ParameterExpression parameter = Expression.Parameter(typeof(TItem), "entity");
            Expression property = formattedPropertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
            UnaryExpression convertedProperty = Expression.Convert(property, typeof(object));
            Expression<Func<TItem, object?>> expression = Expression.Lambda<Func<TItem, object?>>(convertedProperty, parameter);
            if (isDescending && !isQueryOrdered)
            {
                query = query.OrderByDescending(expression);
            }
            else if (!isDescending && !isQueryOrdered)
            {
                query = query.OrderBy(expression);
            }
            else if (isDescending && isQueryOrdered)
            {
                query = ((IOrderedQueryable<TItem>)query).ThenByDescending(expression);
            }
            else if (!isDescending && isQueryOrdered)
            {
                query = ((IOrderedQueryable<TItem>)query).ThenBy(expression);
            }
        }
        return query;
    }
}
