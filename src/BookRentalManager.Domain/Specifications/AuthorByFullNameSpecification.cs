namespace BookRentalManager.Domain.Specifications;

public sealed class AuthorByFullNameSpecification : Specification<Author>
{
    public AuthorByFullNameSpecification(string firstName, string lastName)
    {
        Where = author => author.FullName.FirstName == firstName && author.FullName.LastName == lastName;
        CacheKey = nameof(AuthorByFullNameSpecification) + "-" + firstName + lastName;
    }
}
