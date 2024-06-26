using BookRentalManager.Application.Common;
using BookRentalManager.Application.Dtos;

namespace BookRentalManager.IntegrationTests.Common;

public sealed record AuthorsWithHateoasLinks(List<AuthorWithHateoasLinks> Values, List<HateoasLinkDto> Links);
public sealed record AuthorWithHateoasLinks(string FullName, Guid Id, List<GetBookDto> Books, List<HateoasLinkDto> Links);
