namespace BookRentalManager.Application.Common;

public record GetAllItemsQuery(int PageIndex, int PageSize, string Search, string Sort);
