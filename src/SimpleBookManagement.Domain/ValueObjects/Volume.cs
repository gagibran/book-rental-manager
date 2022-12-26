namespace SimpleBookManagement.Domain.ValueObjects;

public sealed class Volume : ValueObject
{
    public int VolumeNumber { get; }

    private Volume(int volumeNumber)
    {
        VolumeNumber = volumeNumber;
    }

    public static Result<Volume> Create(int volumeNumber)
    {
        if (volumeNumber < 1)
        {
            return Result<Volume>.Fail("The volume number can't be smaller than 1.");
        }
        var volume = new Volume(volumeNumber);
        return Result<Volume>.Success(volume);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return VolumeNumber;
    }
}
