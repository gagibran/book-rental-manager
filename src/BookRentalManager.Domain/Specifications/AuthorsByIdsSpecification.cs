namespace BookRentalManager.Domain.Specifications;

public sealed class AuthorsByIdsSpecification : Specification<Author>
{
    public AuthorsByIdsSpecification(IEnumerable<Guid> ids)
    {
        Where = author => ids.Contains(author.Id);
        CacheKey = nameof(AuthorsByIdsSpecification) + "-";
        foreach (Guid id in ids)
        {
            CacheKey += id.ToString();
        }
    }
}
