namespace BookRentalManager.Application.Books.Queries;

public sealed record GetBooksByQueryParametersExcludingFromAuthorQuery(
    Guid AuthorId,
    int PageIndex,
    int PageSize,
    string SearchParameter,
    string SortParameters)
    : GetAllItemsQuery(PageIndex, PageSize, SearchParameter, SortParameters), IRequest<PaginatedList<GetBookDto>>;
