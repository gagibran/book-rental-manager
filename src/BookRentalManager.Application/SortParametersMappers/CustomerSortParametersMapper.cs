namespace BookRentalManager.Application.SortParametersMappers;

internal sealed class CustomerSortParametersMapper : IMapper<CustomerSortParameters, Result<string>>
{
    public Result<string> Map(CustomerSortParameters authorSortParameters)
    {
        string convertedPropertyNamesSeparatedByComma = string.Empty;
        string[] propertyNames = authorSortParameters.PropertyNamesSeparatedByComma.Split(',');
        foreach (string propertyName in propertyNames)
        {
            var formattedPropertyName = propertyName.Trim();
            if (!authorSortParameters.ExpectedProperties.Contains(formattedPropertyName))
            {
                return Result.Fail<string>(
                    "invalidProperty",
                    $"The property '{formattedPropertyName}' does not exist for '{nameof(GetCustomerDto)}'.");
            }
            if (formattedPropertyName.Contains("FullName"))
            {
                formattedPropertyName = formattedPropertyName.EndsWith("Desc") ? "FullName.CompleteNameDesc" : "FullName.CompleteName";
            }
            else if (formattedPropertyName.Contains("Email"))
            {
                formattedPropertyName = formattedPropertyName.EndsWith("Desc") ? "Email.EmailAddressDesc" : "Email.EmailAddress";
            }
            else if (formattedPropertyName.Contains("PhoneNumber"))
            {
                formattedPropertyName = formattedPropertyName.EndsWith("Desc") ? "PhoneNumber.CompletePhoneNumberDesc" : "PhoneNumber.CompletePhoneNumber";
            }
            else if (formattedPropertyName.Contains("CustomerStatus"))
            {
                formattedPropertyName = formattedPropertyName.EndsWith("Desc") ? "CustomerStatus.CustomerTypeDesc" : "CustomerStatus.CustomerType";
            }
            convertedPropertyNamesSeparatedByComma += formattedPropertyName + ",";
        }
        return Result.Success<string>(convertedPropertyNamesSeparatedByComma.TrimEnd(','));
    }
}
