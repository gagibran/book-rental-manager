namespace BookRentalManager.Application.SortParameters;

public sealed class BookSortParameters
{
    public string[] ExpectedProperties { get; }
    public string PropertyNamesSeparatedByComma { get; }

    public BookSortParameters(string propertyNamesSeparatedByComma)
    {
        ExpectedProperties = new string[]
        {
            "BookTitle",
            "BookTitleDesc",
            "Edition",
            "EditionDesc",
            "Isbn",
            "IsbnDesc",
            "IsAvailable",
            "IsAvailableDesc",
            "CreatedAt",
            "CreatedAtDesc"
        };
        PropertyNamesSeparatedByComma = propertyNamesSeparatedByComma;
    }
}
