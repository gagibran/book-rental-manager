namespace BookRentalManager.Application.Dtos;

public sealed class ReturnCustomerBookByIdDto
{
    public IEnumerable<Guid> BookIds { get; set; }

    public ReturnCustomerBookByIdDto(IEnumerable<Guid> bookIds)
    {
        BookIds = bookIds;
    }
}
