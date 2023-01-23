namespace BookRentalManager.Application.Dtos;

public sealed class GetBookAuthorDto
{
    public Guid Id { get; }
    public string FullName { get; }
    public IReadOnlyList<GetBookAuthorBookDto> Books { get; }

    public GetBookAuthorDto(Guid id, FullName fullName, IReadOnlyList<GetBookAuthorBookDto> books)
    {
        Id = id;
        FullName = fullName.CompleteName;
        Books = books;
    }
}
