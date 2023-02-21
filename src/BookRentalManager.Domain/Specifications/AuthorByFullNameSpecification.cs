namespace BookRentalManager.Domain.Specifications;

public sealed class AuthorByFullNameSpecification : Specification<Author>
{

    public AuthorByFullNameSpecification(string fullName)
    {
        Where = author => author.FullName.CompleteName == fullName;
    }
}
