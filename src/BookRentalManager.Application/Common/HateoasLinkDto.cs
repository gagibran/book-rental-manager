namespace BookRentalManager.Application.Common;

public sealed class HateoasLinkDto(string href, string rel, string method)
{
    public string Href { get; } = href;
    public string Rel { get; } = rel;
    public string Method { get; } = method;
}
