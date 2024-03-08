namespace BookRentalManager.Application.Dtos;

public sealed record GetAuthorFromBookDto(FullName FullName)
{
    public GetAuthorFromBookDto(Author author) : this(author.FullName)
    {
    }
}
