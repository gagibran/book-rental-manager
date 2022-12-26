namespace SimpleBookManagement.Domain.ValueObjects;

public abstract class ValueObject
{
    public abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || GetType() != obj.GetType())
        {
            return false;
        }
        var otherValueObject = (ValueObject)obj;
        return GetEqualityComponents()
            .SequenceEqual(otherValueObject.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(thisValueObject => thisValueObject != null ? thisValueObject.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject left, ValueObject right)
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

    public static bool operator !=(ValueObject left, ValueObject right)
    {
        return !(left == right);
    }
}
