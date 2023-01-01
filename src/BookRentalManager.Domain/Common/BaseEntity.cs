namespace BookRentalManager.Domain.Common;

public class BaseEntity
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}
