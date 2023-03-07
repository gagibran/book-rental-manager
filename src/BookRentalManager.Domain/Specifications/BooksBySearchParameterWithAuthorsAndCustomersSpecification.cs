namespace BookRentalManager.Domain.Specifications;

public sealed class BooksBySearchParameterWithAuthorsAndCustomersSpecification : Specification<Book>
{
    public BooksBySearchParameterWithAuthorsAndCustomersSpecification(string searchParameter, string sortParameters)
    {
        string formattedSearchParameter = searchParameter.Trim().ToLower();
        Where = book => book.BookTitle.ToLower().Contains(formattedSearchParameter)
                || book.Edition.EditionNumber.ToString().Contains(formattedSearchParameter)
                || book.Isbn.IsbnValue.ToLower().Contains(formattedSearchParameter)
                || book.RentedAt.ToString()!.Contains(formattedSearchParameter)
                || book.DueDate.ToString()!.Contains(formattedSearchParameter)
                || book.Customer!.FullName.FirstName.ToLower().Contains(searchParameter)
                || book.Customer!.FullName.LastName.ToLower().Contains(searchParameter)
                || book.Customer.Email.EmailAddress.ToLower().Contains(searchParameter);
        IncludeExpressions.Add(book => book.Authors);
        IncludeExpressions.Add(book => book.Customer!);
        OrderByPropertyName = sortParameters;
    }
}
