namespace BookRentalManager.Application.Dtos;

public sealed class HateoasLinkDto
{
    public string Href { get; }
    public string Rel { get; }
    public string Method { get; }

    public HateoasLinkDto(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
}
