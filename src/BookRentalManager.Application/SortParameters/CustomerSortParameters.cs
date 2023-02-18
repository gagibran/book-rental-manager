namespace BookRentalManager.Application.SortParameters;

public sealed class CustomerSortParameters
{
    public string[] ExpectedProperties { get; }
    public string PropertyNamesSeparatedByComma { get; }

    public CustomerSortParameters(string propertyNamesSeparatedByComma)
    {
        ExpectedProperties = new string[]
        {
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
        };
        PropertyNamesSeparatedByComma = propertyNamesSeparatedByComma;
    }
}
