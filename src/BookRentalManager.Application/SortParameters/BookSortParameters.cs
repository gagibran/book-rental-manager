namespace BookRentalManager.Application.SortParameters;

public sealed class BookSortParameters(string propertyNamesSeparatedByComma)
{
    public string[] ExpectedProperties { get; } =
    [
        "BookTitle",
        "BookTitleDesc",
        "Edition",
        "EditionDesc",
        "Isbn",
        "IsbnDesc",
        "RentedAt",
        "RentedAtDesc",
        "DueDate",
        "DueDateDesc",
        "CreatedAt",
        "CreatedAtDesc"
    ];
    public string PropertyNamesSeparatedByComma { get; } = propertyNamesSeparatedByComma;
}
