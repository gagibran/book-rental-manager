namespace BookRentalManager.Domain.Specifications;

public sealed class BooksBySearchParameterInBooksFromAuthorSpecification : Specification<Book>
{
    public BooksBySearchParameterInBooksFromAuthorSpecification(IReadOnlyList<Book> books, string searchParameter, string sortParameters)
    {
        string formattedSearchParameter = searchParameter.Trim().ToLower();
        Where = book => books.Contains(book)
            && (book.BookTitle.ToLower().Contains(formattedSearchParameter)
                || book.Edition.EditionNumber.ToString().Contains(formattedSearchParameter)
                || book.Isbn.IsbnValue.ToLower().Contains(formattedSearchParameter)
                || book.IsAvailable.ToString().ToLower().Contains(formattedSearchParameter)
                || book.Customer!.FullName.CompleteName.ToLower().Contains(searchParameter)
                || book.Customer.Email.EmailAddress.ToLower().Contains(searchParameter));
        IncludeExpressions.Add(book => book.Authors);
        IncludeExpressions.Add(book => book.Customer!);
        OrderByPropertyName = sortParameters;
    }
}