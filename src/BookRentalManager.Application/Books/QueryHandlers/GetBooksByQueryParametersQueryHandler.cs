namespace BookRentalManager.Application.Books.QueryHandlers;

internal sealed class GetBooksByQueryParametersQueryHandler(
    IRepository<Book> bookRepository,
    ISortParametersMapper sortParametersMapper)
    : IRequestHandler<GetBooksByQueryParametersQuery, PaginatedList<GetBookDto>>
{
    public async Task<Result<PaginatedList<GetBookDto>>> HandleAsync(
        GetBooksByQueryParametersQuery getBooksByQueryParametersQuery,
        CancellationToken cancellationToken)
    {
        Result<string> convertedSortParametersResult = sortParametersMapper.MapBookSortParameters(
            getBooksByQueryParametersQuery.SortParameters);
        if (!convertedSortParametersResult.IsSuccess)
        {
            return Result.Fail<PaginatedList<GetBookDto>>(
                convertedSortParametersResult.ErrorType,
                convertedSortParametersResult.ErrorMessage);
        }
        var booksBySearchParameterWithAuthorsAndCustomersSpecification = new BooksBySearchParameterWithAuthorsAndCustomersSpecification(
            getBooksByQueryParametersQuery.SearchParameter,
            convertedSortParametersResult.Value!);
        PaginatedList<Book> books = await bookRepository.GetAllBySpecificationAsync(
            getBooksByQueryParametersQuery.PageIndex,
            getBooksByQueryParametersQuery.PageSize,
            booksBySearchParameterWithAuthorsAndCustomersSpecification,
            cancellationToken);
        List<GetBookDto> getBookDtos = books
            .Select(book => new GetBookDto(book))
            .ToList();
        var paginatedGetBookDtos = new PaginatedList<GetBookDto>(
            getBookDtos,
            books.TotalAmountOfItems,
            books.TotalAmountOfPages,
            books.PageIndex,
            books.PageSize);
        return Result.Success(paginatedGetBookDtos);
    }
}
