namespace BookRentalManager.Application.Books.Queries;

public sealed record GetBooksByQueryParametersQuery(int PageIndex, int PageSize, string SearchParameter, string SortParameters)
    : GetAllItemsQuery(PageIndex, PageSize, SearchParameter, SortParameters), IRequest<PaginatedList<GetBookDto>>;
