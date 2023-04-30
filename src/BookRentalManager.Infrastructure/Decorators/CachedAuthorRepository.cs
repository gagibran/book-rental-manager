namespace BookRentalManager.Infrastructure.Decorators;

public sealed class CachedAuthorRepository : IRepository<Author>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IMemoryCache _memoryCache;

    public CachedAuthorRepository(IRepository<Author> authorRepository, IMemoryCache memoryCache)
    {
        _authorRepository = authorRepository;
        _memoryCache = memoryCache;
    }

    public async Task<IReadOnlyList<Author>> GetAllBySpecificationAsync(Specification<Author> specification, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(specification.CacheKey))
        {
            throw new ArgumentException($"The specification {specification.GetType()} does not have a cache key defined.");
        }
        IReadOnlyList<Author>? existingAuthorsCache = _memoryCache.Get<IReadOnlyList<Author>>(specification.CacheKey);
        if (existingAuthorsCache is not null)
        {
            return existingAuthorsCache;
        }
        IReadOnlyList<Author> existingAuthors = await _authorRepository.GetAllBySpecificationAsync(specification, cancellationToken);
        return _memoryCache.Set(specification.CacheKey, existingAuthors, TimeSpan.FromMinutes(2));
    }

    public async Task<PaginatedList<Author>> GetAllBySpecificationAsync(
        int pageIndex,
        int pageSize,
        Specification<Author> specification,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(specification.CacheKey))
        {
            throw new ArgumentException($"The specification {specification.GetType()} does not have a cache key defined.");
        }
        PaginatedList<Author>? existingAuthorsCache = _memoryCache.Get<PaginatedList<Author>>(specification.CacheKey);
        if (existingAuthorsCache is not null)
        {
            return existingAuthorsCache;
        }
        PaginatedList<Author> existingAuthors = await _authorRepository.GetAllBySpecificationAsync(pageIndex, pageSize, specification, cancellationToken);
        return _memoryCache.Set(specification.CacheKey, existingAuthors, TimeSpan.FromMinutes(2));
    }

    public async Task<Author?> GetFirstOrDefaultBySpecificationAsync(Specification<Author> specification, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(specification.CacheKey))
        {
            throw new ArgumentException($"The specification {specification.GetType()} does not have a cache key defined.");
        }
        return await _memoryCache.GetOrCreateAsync(specification.CacheKey, cacheEntry =>
        {
            cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
            return _authorRepository.GetFirstOrDefaultBySpecificationAsync(specification, cancellationToken);
        });
    }

    public async Task CreateAsync(Author author, CancellationToken cancellationToken = default)
    {
        await _authorRepository.CreateAsync(author, cancellationToken);
    }

    public async Task DeleteAsync(Author author, CancellationToken cancellationToken = default)
    {
        await _authorRepository.DeleteAsync(author, cancellationToken);
    }

    public async Task UpdateAsync(Author author, CancellationToken cancellationToken = default)
    {
        await _authorRepository.UpdateAsync(author, cancellationToken);
    }
}
