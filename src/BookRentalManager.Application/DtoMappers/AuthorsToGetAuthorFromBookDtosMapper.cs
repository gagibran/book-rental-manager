namespace BookRentalManager.Application.DtoMappers;

internal sealed class AuthorsToGetAuthorFromBookDtosMapper : IMapper<IReadOnlyList<Author>, IReadOnlyList<GetAuthorFromBookDto>>
{
    public IReadOnlyList<GetAuthorFromBookDto> Map(IReadOnlyList<Author> authors)
    {
        return (from author in authors
                select new GetAuthorFromBookDto(author.FullName)).ToList().AsReadOnly();
    }
}
