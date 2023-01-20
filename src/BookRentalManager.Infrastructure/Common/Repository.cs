using System.Linq.Expressions;
using BookRentalManager.Domain.Common;
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

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(
        int pageIndex,
        int totalItemsPerPage,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToReadOnlyPaginatedListAsync(cancellationToken, pageIndex, totalItemsPerPage);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(
        int pageIndex,
        int totalItemsPerPage,
        Specification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = ApplySpecification(_dbSet, specification);
        return await queryable.ToReadOnlyPaginatedListAsync(cancellationToken, pageIndex, totalItemsPerPage);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.FirstOrDefaultAsync(entity => entity.Id == id);
    }

    public async Task<TEntity?> GetByIdAsync(
        Guid id,
        Specification<TEntity> specification,
        CancellationToken cancellationToken)
    {
        return await ApplySpecification(_dbSet, specification)
            .FirstOrDefaultAsync(entity => entity.Id == id);
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

    public IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> queryable, Specification<TEntity> specification)
    {
        IQueryable<TEntity> currentQueryable = queryable;
        if (specification.WhereExpression is not null)
        {
            currentQueryable = currentQueryable.Where(specification.WhereExpression);
        }
        foreach (Expression<Func<TEntity, object>> includeExpression in specification.IncludeExpressions)
        {
            currentQueryable = currentQueryable.Include(includeExpression);
        }
        return currentQueryable;
    }
}
