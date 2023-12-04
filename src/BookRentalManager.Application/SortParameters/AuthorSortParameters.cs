namespace BookRentalManager.Application.SortParameters;

public sealed class AuthorSortParameters(string propertyNamesSeparatedByComma)
{
    public string[] ExpectedProperties { get; } =
    [
        "FullName",
        "FullNameDesc",
        "CreatedAt",
        "CreatedAtDesc"
    ];
    public string PropertyNamesSeparatedByComma { get; } = propertyNamesSeparatedByComma;
}
