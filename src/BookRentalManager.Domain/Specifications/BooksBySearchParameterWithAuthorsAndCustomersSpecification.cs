namespace BookRentalManager.Domain.Specifications;

public sealed class BooksBySearchParameterWithAuthorsAndCustomersSpecification : Specification<Book>
{
    public BooksBySearchParameterWithAuthorsAndCustomersSpecification(string searchParameter, string sortParameters)
    {
        string formattedSearchParameter = searchParameter.Trim().ToLower();
        Where = book => book.BookTitle.ToLower().Contains(formattedSearchParameter)
                || book.Edition.EditionNumber.ToString().Contains(formattedSearchParameter)
                || book.Isbn.IsbnValue.ToLower().Contains(formattedSearchParameter)
                || book.IsAvailable.ToString().ToLower().Contains(formattedSearchParameter)
                || book.Customer!.FullName.CompleteName.ToLower().Contains(searchParameter)
                || book.Customer.Email.EmailAddress.ToLower().Contains(searchParameter);
        IncludeExpressions.Add(book => book.Authors);
        IncludeExpressions.Add(book => book.Customer!);
        OrderByPropertyName = sortParameters;
    }
}