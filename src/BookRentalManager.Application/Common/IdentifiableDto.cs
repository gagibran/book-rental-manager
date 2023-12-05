namespace BookRentalManager.Application.Common;

public abstract class IdentifiableDto(Guid id)
{
    public Guid Id { get; } = id;
}
