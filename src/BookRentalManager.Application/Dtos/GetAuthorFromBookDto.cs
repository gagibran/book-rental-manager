namespace BookRentalManager.Application.Dtos;

[method: JsonConstructor]
public sealed record GetAuthorFromBookDto(FullName FullName)
{
    public GetAuthorFromBookDto(Author author) : this(author.FullName)
    {
    }
}
