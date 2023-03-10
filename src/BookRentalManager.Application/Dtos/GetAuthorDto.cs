namespace BookRentalManager.Application.Dtos;

public sealed class GetAuthorDto
{
    public Guid Id { get; }
    public string FullName { get; }
    public IReadOnlyList<GetBookFromAuthorDto> Books { get; }

    public GetAuthorDto(Guid id, FullName fullName, IReadOnlyList<GetBookFromAuthorDto> books)
    {
        Id = id;
        FullName = fullName.ToString();
        Books = books;
    }
}
