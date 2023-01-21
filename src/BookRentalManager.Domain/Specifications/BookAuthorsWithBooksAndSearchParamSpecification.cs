namespace BookRentalManager.Domain.Specifications;

public sealed class BookAuthorsWithBooksAndSearchParamSpecification : Specification<BookAuthor>
{
    public BookAuthorsWithBooksAndSearchParamSpecification(string searchParameter)
    {
        Where = bookAuthor => bookAuthor.FullName.CompleteName.ToLower().Contains(searchParameter.Trim().ToLower());
        IncludeExpressions.Add(bookAuthor => bookAuthor.Books);
    }
}
