namespace BookRentalManager.Domain.Specifications;

public sealed class BookAuthorsBySearchParameterSpecification : Specification<BookAuthor>
{
    public BookAuthorsBySearchParameterSpecification(string searchParameter)
    {
        Where = bookAuthor => bookAuthor.FullName.CompleteName.ToLower().Contains(searchParameter.Trim().ToLower());
        IncludeExpressions.Add(bookAuthor => bookAuthor.Books);
    }
}
