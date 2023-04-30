namespace BookRentalManager.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task<IReadOnlyList<TEntity>> GetAllBySpecificationAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<PaginatedList<TEntity>> GetAllBySpecificationAsync(
        int pageIndex,
        int pageSize,
        Specification<TEntity> specification,
        CancellationToken cancellationToken = default);
    Task<TEntity?> GetFirstOrDefaultBySpecificationAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default);
    Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
