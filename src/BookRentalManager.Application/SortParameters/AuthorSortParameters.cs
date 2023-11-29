namespace BookRentalManager.Application.SortParameters;

public sealed class AuthorSortParameters
{
    public string[] ExpectedProperties { get; }
    public string PropertyNamesSeparatedByComma { get; }

    public AuthorSortParameters(string propertyNamesSeparatedByComma)
    {
        ExpectedProperties =
        [
            "FullName",
            "FullNameDesc",
            "CreatedAt",
            "CreatedAtDesc"
        ];
        PropertyNamesSeparatedByComma = propertyNamesSeparatedByComma;
    }
}
