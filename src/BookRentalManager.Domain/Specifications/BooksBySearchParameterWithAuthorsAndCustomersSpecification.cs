namespace BookRentalManager.Domain.Specifications;

public sealed class BooksBySearchParameterWithAuthorsAndCustomersSpecification : Specification<Book>
{
    public BooksBySearchParameterWithAuthorsAndCustomersSpecification(string searchParameter, string sortParameters)
    {
        string formattedSearchParameter = searchParameter.Trim().ToLower();
        Where = book => book.BookTitle.Contains(formattedSearchParameter, StringComparison.CurrentCultureIgnoreCase)
                || book.Edition.EditionNumber.ToString().Contains(formattedSearchParameter)
                || book.Isbn.IsbnValue.Contains(formattedSearchParameter, StringComparison.CurrentCultureIgnoreCase)
                || book.RentedAt.ToString()!.Contains(formattedSearchParameter)
                || book.DueDate.ToString()!.Contains(formattedSearchParameter)
                || book.Customer!.FullName.FirstName.Contains(searchParameter, StringComparison.CurrentCultureIgnoreCase)
                || book.Customer!.FullName.LastName.Contains(searchParameter, StringComparison.CurrentCultureIgnoreCase)
                || book.Customer.Email.EmailAddress.Contains(searchParameter, StringComparison.CurrentCultureIgnoreCase);
        IncludeExpressions.Add(book => book.Authors);
        IncludeExpressions.Add(book => book.Customer!);
        OrderByPropertyName = sortParameters;
    }
}
