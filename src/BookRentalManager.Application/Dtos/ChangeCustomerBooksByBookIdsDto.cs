namespace BookRentalManager.Application.Dtos;

public sealed class ChangeCustomerBooksByBookIdsDto
{
    public IEnumerable<Guid> BookIds { get; set; }

    public ChangeCustomerBooksByBookIdsDto(IEnumerable<Guid> bookIds)
    {
        BookIds = bookIds;
    }
}
