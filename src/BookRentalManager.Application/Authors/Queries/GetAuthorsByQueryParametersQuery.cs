namespace BookRentalManager.Application.Authors.Queries;

public sealed record GetAuthorsByQueryParametersQuery(int PageIndex, int PageSize, string SearchParameter, string SortParameters)
    : GetAllItemsQuery(PageIndex, PageSize, SearchParameter, SortParameters), IRequest<PaginatedList<GetAuthorDto>>;
