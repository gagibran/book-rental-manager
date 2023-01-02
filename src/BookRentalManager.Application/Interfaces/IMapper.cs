namespace BookRentalManager.Application.Interfaces;

public interface IMapper<TEntity, TDto>
{
    public TDto Map(TEntity entity);
}
