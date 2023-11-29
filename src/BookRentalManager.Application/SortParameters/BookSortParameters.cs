namespace BookRentalManager.Application.SortParameters;

public sealed class BookSortParameters
{
    public string[] ExpectedProperties { get; }
    public string PropertyNamesSeparatedByComma { get; }

    public BookSortParameters(string propertyNamesSeparatedByComma)
    {
        ExpectedProperties =
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
        PropertyNamesSeparatedByComma = propertyNamesSeparatedByComma;
    }
}
