namespace BookRentalManager.Application.Dtos;

public sealed record ChangeCustomerBooksByBookIdsDto(IEnumerable<Guid> BookIds);
