namespace BookRentalManager.Api.QueryParameters;

/// <summary>
/// Parameters to refine the returned paginated list of objects.
/// </summary>
public sealed record GetAllItemsQueryParameters
{
    /// <summary>
    /// The page index of the returned list of items. The default value is 1.
    /// </summary>
    public int PageIndex { get; init; } = 1;

    /// <summary>
    /// The amount of items that will be returned in one page. The default is 50.
    /// </summary>
    public int PageSize { get; init; } = 50;

    /// <summary>
    /// The search string that will be used to refine the search.
    /// </summary>
    public string SearchQuery { get; init; } = "";

    /// <summary>
    /// The sort parameters accepted by the GET request (see the "Allowed to sort by" section). The default is "CreatedAt".
    /// </summary>
    public string SortBy { get; set; } = "CreatedAt";
}
