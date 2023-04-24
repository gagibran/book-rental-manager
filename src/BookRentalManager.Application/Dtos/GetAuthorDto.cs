namespace BookRentalManager.Application.Dtos;

public sealed class GetAuthorDto : SingleResourceBaseDto
{
    public string FullName { get; }
    public IReadOnlyList<GetBookFromAuthorDto> Books { get; }

    public GetAuthorDto(Guid id, FullName fullName, IReadOnlyList<GetBookFromAuthorDto> books) : base(id)
    {
        FullName = fullName.ToString();
        Books = books;
    }
}
