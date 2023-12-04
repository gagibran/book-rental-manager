namespace BookRentalManager.Application.Common;

public class IdentifiableDto(Guid id)
{
    public Guid Id { get; } = id;
}
