namespace BookRentalManager.Application.SortParametersMappers;

internal sealed class BookSortParametersMapper : IMapper<BookSortParameters, string>
{
    public string Map(BookSortParameters authorSortParameters)
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
            if (formattedPropertyName.Contains("Edition"))
            {
                formattedPropertyName = formattedPropertyName.EndsWith("Desc") ? "Edition.EditionNumberDesc" : "Edition.EditionNumber";
            }
            else if (formattedPropertyName.Contains("Isbn"))
            {
                formattedPropertyName = formattedPropertyName.EndsWith("Desc") ? "Isbn.IsbnValueDesc" : "Isbn.IsbnValue";
            }
            convertedPropertyNamesSeparatedByComma += formattedPropertyName + ",";
        }
        return convertedPropertyNamesSeparatedByComma.TrimEnd(',');
    }
}
