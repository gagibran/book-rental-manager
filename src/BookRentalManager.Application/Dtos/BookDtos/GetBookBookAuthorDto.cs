namespace BookRentalManager.Application.Dtos.BookDtos;

public sealed class GetBookBookAuthorDto
{
    public string FullName { get; }

    public GetBookBookAuthorDto(FullName fullName)
    {
        FullName = fullName.CompleteName;
    }
}
