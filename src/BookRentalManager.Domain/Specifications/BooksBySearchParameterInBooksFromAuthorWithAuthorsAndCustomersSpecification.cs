namespace BookRentalManager.Domain.Specifications;

public sealed class BooksBySearchParameterWithAuthorsAndCustomersInBooksFromAuthorSpecification : Specification<Book>
{
    public BooksBySearchParameterWithAuthorsAndCustomersInBooksFromAuthorSpecification(
        IReadOnlyList<Book> books,
        string searchParameter,
        string sortParameters)
    {
        string formattedSearchParameter = searchParameter.Trim().ToLower();
        Where = book => books.Contains(book)
            && (book.BookTitle.ToLower().Contains(formattedSearchParameter)
                || book.Edition.EditionNumber.ToString().Contains(formattedSearchParameter)
                || book.Isbn.IsbnValue.ToLower().Contains(formattedSearchParameter)
                || book.IsAvailable.ToString().ToLower().Contains(formattedSearchParameter)
                || book.Customer!.FullName.FirstName.ToLower().Contains(searchParameter)
                || book.Customer!.FullName.LastName.ToLower().Contains(searchParameter)
                || book.Customer.Email.EmailAddress.ToLower().Contains(searchParameter));
        IncludeExpressions.Add(book => book.Authors);
        IncludeExpressions.Add(book => book.Customer!);
        OrderByPropertyName = sortParameters;
    }
}
