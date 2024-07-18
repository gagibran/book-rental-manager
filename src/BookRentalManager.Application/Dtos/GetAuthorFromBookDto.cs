namespace BookRentalManager.Application.Dtos;

[method: JsonConstructor]
public sealed record GetAuthorFromBookDto(string FullName)
{
    public GetAuthorFromBookDto(Author author) : this(author.FullName.ToString())
    {
    }
}
