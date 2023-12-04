namespace BookRentalManager.Application.Dtos;

public sealed class GetAuthorDto(Guid id, FullName fullName, IReadOnlyList<GetBookFromAuthorDto> books) : IdentifiableDto(id)
{
    public string FullName { get; } = fullName.ToString();
    public IReadOnlyList<GetBookFromAuthorDto> Books { get; } = books;
}
