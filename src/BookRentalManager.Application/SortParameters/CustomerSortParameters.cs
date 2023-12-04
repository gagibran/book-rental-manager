namespace BookRentalManager.Application.SortParameters;

public sealed class CustomerSortParameters(string propertyNamesSeparatedByComma)
{
    public string[] ExpectedProperties { get; } =
    [
        "FullName",
        "FullNameDesc",
        "Email",
        "EmailDesc",
        "PhoneNumber",
        "PhoneNumberDesc",
        "CustomerStatus",
        "CustomerStatusDesc",
        "CustomerPoints",
        "CustomerPointsDesc",
        "CreatedAt",
        "CreatedAtDesc"
    ];
    public string PropertyNamesSeparatedByComma { get; } = propertyNamesSeparatedByComma;
}
