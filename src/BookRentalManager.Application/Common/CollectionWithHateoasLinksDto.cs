using System.Dynamic;

namespace BookRentalManager.Application.Common;

public sealed record CollectionWithHateoasLinksDto(PaginatedList<ExpandoObject> Values, List<HateoasLinkDto> Links);
