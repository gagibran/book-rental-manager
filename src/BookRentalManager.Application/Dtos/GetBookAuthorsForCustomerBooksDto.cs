namespace BookRentalManager.Application.Dtos;

public sealed class GetBookAuthorsForCustomerBooksDto
{
    public FullName FullName { get; }

    public GetBookAuthorsForCustomerBooksDto(FullName fullName)
    {
        FullName = fullName;
    }
}
