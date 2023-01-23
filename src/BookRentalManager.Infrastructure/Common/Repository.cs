using BookRentalManager.Infrastructure.Data;
using BookRentalManager.Infrastructure.Extensions;

namespace BookRentalManager.Infrastructure.Common;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly BookRentalManagerDbContext _bookRentalManagerDbContext;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(BookRentalManagerDbContext bookRentalManagerDbContext)
    {
        _bookRentalManagerDbContext = bookRentalManagerDbContext;
        _dbSet = bookRentalManagerDbContext.Set<TEntity>();
    }

    public async Task<IReadOnlyList<TEntity>> GetAllBySpecificationAsync(
        int pageIndex,
        int totalItemsPerPage,
        Specification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = ApplySpecification(_dbSet, specification);
        return await query.ToReadOnlyPaginatedListAsync(cancellationToken, pageIndex, totalItemsPerPage);
    }

    public async Task<TEntity?> GetFirstOrDefaultBySpecificationAsync(
        Specification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(_dbSet, specification).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity);
        await SaveAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await SaveAsync(cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await SaveAsync(cancellationToken);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _bookRentalManagerDbContext.SaveChangesAsync();
    }

    public IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> query, Specification<TEntity> specification)
    {
        return SpecificationEvaluator.GetQuery<TEntity>(query, specification);
    }
}
