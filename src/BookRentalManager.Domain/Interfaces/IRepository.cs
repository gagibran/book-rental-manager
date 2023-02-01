namespace BookRentalManager.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task<PaginatedList<TEntity>> GetAllBySpecificationAsync(
        int pageIndex,
        int totalItemsPerPage,
        Specification<TEntity> specification,
        CancellationToken cancellationToken);
    Task<TEntity?> GetFirstOrDefaultBySpecificationAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default);
    Task CreateAsync(TEntity entity, CancellationToken cancellationToken);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    Task SaveAsync(CancellationToken cancellationToken);
    IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> query, Specification<TEntity> specification);
}
