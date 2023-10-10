namespace BookRentalManager.Domain.Common;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || GetType() != obj.GetType())
        {
            return false;
        }
        var otherValueObject = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(otherValueObject.GetEqualityComponents());
    }

    public static bool operator ==(ValueObject left, ValueObject right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ValueObject left, ValueObject right)
    {
        return !left.Equals(right);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(thisValueObject => thisValueObject?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public bool Equals(ValueObject? other)
    {
        return Equals(other);
    }
}
