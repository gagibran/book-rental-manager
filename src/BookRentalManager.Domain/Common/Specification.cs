namespace BookRentalManager.Domain.Common;

public abstract class Specification<TEntity> where TEntity : Entity
{
    public abstract Expression<Func<TEntity, bool>> ToExpression();

    public bool IsSatisfiedBy(TEntity entity)
    {
        Func<TEntity, bool> function = ToExpression().Compile();
        return function(entity);
    }
}
