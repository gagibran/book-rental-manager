namespace BookRentalManager.Application.SortParametersMappers;

internal sealed class AuthorSortParametersMapper : IMapper<AuthorSortParameters, string>
{
    public string Map(AuthorSortParameters authorSortParameters)
    {
        string convertedPropertyNamesSeparatedByComma = string.Empty;
        string[] propertyNames = authorSortParameters.PropertyNamesSeparatedByComma.Split(',');
        foreach (string propertyName in propertyNames)
        {
            var formattedPropertyName = propertyName.Trim();
            if (!authorSortParameters.ExpectedProperties.Contains(formattedPropertyName))
            {
                continue;
            }
            if (formattedPropertyName.Contains("FullName"))
            {
                formattedPropertyName = formattedPropertyName.EndsWith("Desc") ? "FullName.CompleteNameDesc" : "FullName.CompleteName";
            }
            convertedPropertyNamesSeparatedByComma += formattedPropertyName + ",";
        }
        return convertedPropertyNamesSeparatedByComma.TrimEnd(',');
    }
}
