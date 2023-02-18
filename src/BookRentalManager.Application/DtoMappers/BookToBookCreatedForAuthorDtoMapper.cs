namespace BookRentalManager.Application.DtoMappers;

internal sealed class BookToBookCreatedForAuthorDtoMapper : IMapper<Book, BookCreatedForAuthorDto>
{
    public BookCreatedForAuthorDto Map(Book book)
    {
        return new BookCreatedForAuthorDto(
            book.Id,
            book.BookTitle,
            book.Edition.EditionNumber,
            book.Isbn.IsbnValue);
    }
}
