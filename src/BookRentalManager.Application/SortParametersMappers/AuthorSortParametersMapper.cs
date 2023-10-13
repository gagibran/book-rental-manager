namespace BookRentalManager.Application.SortParametersMappers;

internal sealed class AuthorSortParametersMapper : IMapper<AuthorSortParameters, Result<string>>
{
    public Result<string> Map(AuthorSortParameters authorSortParameters)
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
}
