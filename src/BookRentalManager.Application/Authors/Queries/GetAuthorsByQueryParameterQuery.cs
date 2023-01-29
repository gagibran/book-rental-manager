namespace BookRentalManager.Application.Authors.Queries;

public sealed class GetAuthorsByQueryParameterQuery : IQuery<IReadOnlyList<GetAuthorDto>>
{
    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }
    public string SearchParameter { get; }

    public GetAuthorsByQueryParameterQuery(int pageIndex, int totalItemsPerPage, string searchParameter)
    {
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage;
        SearchParameter = searchParameter;
    }
}
