namespace BookRentalManager.Application.DtoMappers;

public sealed class BookToPatchBookTitleEditionAndIsbnByIdDtoMapper : IMapper<Book, PatchBookTitleEditionAndIsbnByIdDto>
{
    public PatchBookTitleEditionAndIsbnByIdDto Map(Book book)
    {
        return new PatchBookTitleEditionAndIsbnByIdDto(
            book.BookTitle,
            book.Edition.EditionNumber,
            book.Isbn.IsbnValue);
    }
}
