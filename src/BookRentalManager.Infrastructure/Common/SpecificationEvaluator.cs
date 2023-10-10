using System.Linq.Expressions;
using BookRentalManager.Domain.Extensions;

namespace BookRentalManager.Infrastructure.Common;

public static class SpecificationEvaluator
{
    public static IQueryable<TEntity> GetQuery<TEntity>(IQueryable<TEntity> query, Specification<TEntity> specification)
        where TEntity : Entity
    {
        IQueryable<TEntity> currentQuery = query;
        if (specification.Where is not null)
        {
            currentQuery = currentQuery.Where(specification.Where);
        }
        if (specification.OrderByPropertyName is not null)
        {
            currentQuery = currentQuery.OrderByPropertyName(specification.OrderByPropertyName);
        }
        foreach (Expression<Func<TEntity, object>> includeExpression in specification.IncludeExpressions)
        {
            currentQuery = currentQuery.Include(includeExpression);
        }
        return currentQuery;
    }
}
