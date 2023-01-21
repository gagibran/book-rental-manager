namespace BookRentalManager.Application.Dtos.BookDtos;

public sealed class GetRentedByDto
{
    public string? FullName { get; }
    public string? Email { get; }

    public GetRentedByDto()
    {
    }

    public GetRentedByDto(FullName fullName, Email email)
    {
        FullName = fullName.CompleteName;
        Email = email.EmailAddress;
    }
}