namespace BookRentalManager.Application.Dtos;

public sealed class GetBookAuthorsForCustomerBooksDto
{
    public string FullName { get; }

    public GetBookAuthorsForCustomerBooksDto(FullName fullName)
    {
        FullName = fullName.CompleteName;
    }
}
