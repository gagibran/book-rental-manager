namespace BookRentalManager.Api.QueryParameters;

public sealed record GetAllItemsQueryParameters
{
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 50;
    public string SearchQuery { get; init; } = "";
    public string SortBy { get; set; } = "CreatedAt";
}
