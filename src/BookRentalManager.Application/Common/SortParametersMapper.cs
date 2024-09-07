namespace BookRentalManager.Application.Common;

public sealed class SortParametersMapper : ISortParametersMapper
{
    private readonly string[] _expectedAuthorProperties;
    private readonly string[] _expectedBookProperties;
    private readonly string[] _expectedCustomerProperties;

    public SortParametersMapper()
    {
        _expectedAuthorProperties =
        [
            "FullName",
            "FullNameDesc",
            "CreatedAt",
            "CreatedAtDesc"
        ];
        _expectedBookProperties =
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
        _expectedCustomerProperties = 
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
    }

    public Result<string> MapAuthorSortParameters(string propertyNamesSeparatedByComma)
    {
        string convertedPropertyNamesSeparatedByComma = string.Empty;
        string[] propertyNames = propertyNamesSeparatedByComma.Split(',');
        foreach (string propertyName in propertyNames)
        {
            var formattedPropertyName = propertyName.Trim();
            if (!_expectedAuthorProperties.Contains(formattedPropertyName))
            {
                return Result.Fail<string>(
                    "invalidProperty",
                    $"The property '{formattedPropertyName}' does not exist for '{nameof(GetAuthorDto)}'.");
            }
            if (formattedPropertyName.Contains("FullName"))
            {
                formattedPropertyName = formattedPropertyName.EndsWith("Desc") ? "FullName.FirstNameDesc,FullName.LastNameDesc" : "FullName.FirstName,FullName.LastName";
            }
            convertedPropertyNamesSeparatedByComma += formattedPropertyName + ",";
        }
        return Result.Success(convertedPropertyNamesSeparatedByComma.TrimEnd(','));
    }

    public Result<string> MapBookSortParameters(string propertyNamesSeparatedByComma)
    {
        string convertedPropertyNamesSeparatedByComma = string.Empty;
        string[] propertyNames = propertyNamesSeparatedByComma.Split(',');
        foreach (string propertyName in propertyNames)
        {
            var formattedPropertyName = propertyName.Trim();
            if (!_expectedBookProperties.Contains(formattedPropertyName))
            {
                return Result.Fail<string>(
                    "invalidProperty",
                    $"The property '{formattedPropertyName}' does not exist for '{nameof(GetBookDto)}'.");
            }
            if (formattedPropertyName.Contains("BookTitle"))
            {
                formattedPropertyName = formattedPropertyName.EndsWith("Desc") ? "BookTitle.TitleDesc" : "BookTitle.Title";
            }
            else if (formattedPropertyName.Contains("Edition"))
            {
                formattedPropertyName = formattedPropertyName.EndsWith("Desc") ? "Edition.EditionNumberDesc" : "Edition.EditionNumber";
            }
            else if (formattedPropertyName.Contains("Isbn"))
            {
                formattedPropertyName = formattedPropertyName.EndsWith("Desc") ? "Isbn.IsbnValueDesc" : "Isbn.IsbnValue";
            }
            convertedPropertyNamesSeparatedByComma += formattedPropertyName + ",";
        }
        return Result.Success(convertedPropertyNamesSeparatedByComma.TrimEnd(','));
    }

    public Result<string> MapCustomerSortParameters(string propertyNamesSeparatedByComma)
    {
        string convertedPropertyNamesSeparatedByComma = string.Empty;
        foreach (string propertyName in propertyNamesSeparatedByComma.Split(','))
        {
            string formattedPropertyName = propertyName.Trim();
            if (!_expectedCustomerProperties.Contains(formattedPropertyName))
            {
                return Result.Fail<string>(
                    "invalidProperty",
                    $"The property '{formattedPropertyName}' does not exist for '{nameof(GetCustomerDto)}'.");
            }
            if (formattedPropertyName.Contains("FullName"))
            {
                formattedPropertyName = formattedPropertyName.EndsWith("Desc") ? "FullName.FirstNameDesc,FullName.LastNameDesc" : "FullName.FirstName,FullName.LastName";
            }
            else if (formattedPropertyName.Contains("Email"))
            {
                formattedPropertyName = formattedPropertyName.EndsWith("Desc") ? "Email.EmailAddressDesc" : "Email.EmailAddress";
            }
            else if (formattedPropertyName.Contains("PhoneNumber"))
            {
                formattedPropertyName = formattedPropertyName.EndsWith("Desc") ? "PhoneNumber.AreaCodeDesc,PhoneNumber.PrefixAndLineNumberDesc" : "PhoneNumber.AreaCode,PhoneNumber.PrefixAndLineNumber";
            }
            else if (formattedPropertyName.Contains("CustomerStatus"))
            {
                formattedPropertyName = formattedPropertyName.EndsWith("Desc") ? "CustomerStatus.CustomerTypeDesc" : "CustomerStatus.CustomerType";
            }
            convertedPropertyNamesSeparatedByComma += formattedPropertyName + ",";
        }
        return Result.Success(convertedPropertyNamesSeparatedByComma.TrimEnd(','));
    }
}
