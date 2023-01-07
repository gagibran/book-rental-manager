namespace BookRentalManager.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task<IReadOnlyList<TEntity>> GetPaginatedAllAsync(
        int pageIndex,
        int totalItemsPerPage, CancellationToken cancellationToken
    );
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TEntity?> GetBySpecificationAsync(
        Specification<TEntity> specification,
        CancellationToken cancellationToken
    );
    Task CreateAsync(TEntity entity, CancellationToken cancellationToken);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    Task SaveAsync(CancellationToken cancellationToken);
}
