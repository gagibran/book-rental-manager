using System.Dynamic;

namespace BookRentalManager.Application.Common;

public sealed class CollectionWithHateoasLinksDto(PaginatedList<ExpandoObject> values, List<HateoasLinkDto> links)
{
    public PaginatedList<ExpandoObject> Values { get; } = values;
    public List<HateoasLinkDto> Links { get; } = links;
}
