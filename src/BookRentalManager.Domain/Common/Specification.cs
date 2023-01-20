namespace BookRentalManager.Domain.Common;

public abstract class Specification<TEntity> where TEntity : Entity
{
    private readonly List<Expression<Func<TEntity, object>>> _includeExpressions;

    public Expression<Func<TEntity, bool>>? WhereExpression { get; private set; }
    public IReadOnlyList<Expression<Func<TEntity, object>>> IncludeExpressions => _includeExpressions.AsReadOnly();

    protected Specification()
    {
        _includeExpressions = new();
    }

    protected void Include(Expression<Func<TEntity, object>> includeExpression)
    {
        _includeExpressions.Add(includeExpression);
    }

    protected void Where(Expression<Func<TEntity, bool>> whereExpression)
    {
        WhereExpression = whereExpression;
    }

    public bool IsSatisfiedBy(TEntity entity)
    {
        if (WhereExpression is not null)
        {
            return WhereExpression.Compile().Invoke(entity);
        }
        return false;
    }
}
