namespace BookRentalManager.Application.Dtos;

public sealed class GetBookAuthorDto
{
    public Guid Id { get; }
    public string FullName { get; }

    public GetBookAuthorDto(Guid id, FullName fullName)
    {
        Id = id;
        FullName = fullName.CompleteName;
    }
}
