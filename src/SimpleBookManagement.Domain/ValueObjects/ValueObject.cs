namespace SimpleBookManagement.Domain.ValueObjects;

public abstract class ValueObject
{
    public abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }
        var otherValueObject = (ValueObject)obj;
        IEnumerable<object> otherEqualityComponents = otherValueObject.GetEqualityComponents();
        IEnumerable<object> thisEqualityComponents = GetEqualityComponents();
        return thisEqualityComponents.SequenceEqual(otherEqualityComponents);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject left, ValueObject right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ValueObject left, ValueObject right)
    {
        return !left.Equals(right);
    }
}
