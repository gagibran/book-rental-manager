namespace BookRentalManager.Application.DtoMappers;

internal sealed class AuthorToAuthorCreatedDtoMapper : IMapper<Author, AuthorCreatedDto>
{
    public AuthorCreatedDto Map(Author author)
    {
        return new AuthorCreatedDto(author.Id, author.FullName.FirstName, author.FullName.LastName);
    }
}
