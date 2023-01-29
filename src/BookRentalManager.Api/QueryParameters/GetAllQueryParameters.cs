namespace BookRentalManager.Api.QueryParameters;

public sealed record GetAllQueryParameters
{
    public int PageIndex { get; init; } = 1;
    public int TotalItemsPerPage { get; init; } = 50;
    public string Search { get; init; } = "";
}
