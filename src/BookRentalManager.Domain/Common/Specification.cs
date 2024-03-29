namespace BookRentalManager.Domain.Common;

public abstract class Specification<TEntity> where TEntity : Entity
{
    public Expression<Func<TEntity, bool>>? Where { get; protected set; }
    public string? OrderByPropertyName { get; protected set; }
    public string? CacheKey { get; protected set; }
    public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; }

    protected Specification()
    {
        IncludeExpressions = new();
    }

    public bool IsSatisfiedBy(TEntity entity)
    {
        if (Where is not null)
        {
            return Where.Compile().Invoke(entity);
        }
        return false;
    }
}
