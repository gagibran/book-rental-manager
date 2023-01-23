namespace BookRentalManager.Application.Dtos;

public sealed class GetBookBookAuthorDto
{
    public string FullName { get; }

    public GetBookBookAuthorDto(FullName fullName)
    {
        FullName = fullName.CompleteName;
    }
}
