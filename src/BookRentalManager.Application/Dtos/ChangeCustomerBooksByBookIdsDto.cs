namespace BookRentalManager.Application.Dtos;

public sealed class ChangeCustomerBooksByBookIdsDto(IEnumerable<Guid> bookIds)
{
    public IEnumerable<Guid> BookIds { get; set; } = bookIds;
}
