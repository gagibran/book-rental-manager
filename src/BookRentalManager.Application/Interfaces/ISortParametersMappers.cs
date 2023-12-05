namespace BookRentalManager.Application.Interfaces;

public interface ISortParametersMapper
{
    Result<string> MapAuthorSortParameters(string propertyNamesSeparatedByComma);
    Result<string> MapBookSortParameters(string propertyNamesSeparatedByComma);
    Result<string> MapCustomerSortParameters(string propertyNamesSeparatedByComma);
}
