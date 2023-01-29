namespace BookRentalManager.Domain.ValueObjects;

public sealed class Edition : ValueObject
{
    public int EditionNumber { get; }

    private Edition()
    {
        EditionNumber = default;
    }

    private Edition(int editionNumber)
    {
        EditionNumber = editionNumber;
    }

    public static Result<Edition> Create(int editionNumber)
    {
        if (editionNumber < 1)
        {
            return Result.Fail<Edition>(nameof(Create), "The edition number can't be smaller than 1.");
        }
        var edition = new Edition(editionNumber);
        return Result.Success<Edition>(edition);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return EditionNumber;
    }
}
