using System.Dynamic;

namespace BookRentalManager.Application.Common;

public sealed class CollectionWithHateoasLinksDto
{
    public PaginatedList<ExpandoObject> Values { get; }
    public List<HateoasLinkDto> Links { get; }

    public CollectionWithHateoasLinksDto(PaginatedList<ExpandoObject> values, List<HateoasLinkDto> links)
    {
        Values = values;
        Links = links;
    }
}
