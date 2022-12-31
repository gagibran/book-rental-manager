namespace SimpleBookManagement.Domain.ValueObjects;

public sealed class Volume : ValueObject
{
    public int VolumeNumber { get; } = default!;

    public Volume()
    {
    }

    private Volume(int volumeNumber)
    {
        VolumeNumber = volumeNumber;
    }

    public static Result<Volume> Create(int volumeNumber)
    {
        if (volumeNumber < 1)
        {
            return Result.Fail<Volume>("The volume number can't be smaller than 1.");
        }
        var volume = new Volume(volumeNumber);
        return Result.Success<Volume>(volume);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return VolumeNumber;
    }
}
