namespace SimpleBookManagement.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? UpdatedAt
    {
        get => DateTime.Now;
    }
}
