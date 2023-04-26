namespace BookRentalManager.Application.Common;

public class IdentifiableDto
{
    public Guid Id { get; }

    public IdentifiableDto(Guid id)
    {
        Id = id;
    }
}
