namespace BookRentalManager.Domain.Specifications;

public sealed class BookAuthorsWithSearchParamSpecification : Specification<BookAuthor>
{
    public BookAuthorsWithSearchParamSpecification(string searchParameter)
    {
        Where = bookAuthor => bookAuthor.FullName.CompleteName.ToLower().Contains(searchParameter.Trim().ToLower());
        IncludeExpressions.Add(bookAuthor => bookAuthor.Books);
    }
}
