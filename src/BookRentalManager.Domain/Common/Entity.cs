namespace BookRentalManager.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}
