namespace BookRentalManager.Application.Dtos;

public sealed class GetAuthorFromBookDto
{
    public string FullName { get; }

    public GetAuthorFromBookDto(FullName fullName)
    {
        FullName = fullName.ToString();
    }
}
