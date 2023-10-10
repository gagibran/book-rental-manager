namespace BookRentalManager.Domain.Common;

public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; }
    public DateTime CreatedAt { get; }

    public Entity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || GetType() != obj.GetType())
        {
            return false;
        }
        var otherEntity = (Entity)obj;
        return Id == otherEntity.Id;
    }


    public static bool operator ==(Entity left, Entity right)
    {
        if (left is null && right is null)
        {
            return true;
        }
        if (left is null || right is null)
        {
            return false;
        }
        return left.Equals(right);
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Entity? other)
    {
        return Equals((object?)other);
    }
}
