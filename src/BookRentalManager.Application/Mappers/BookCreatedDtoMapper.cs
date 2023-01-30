namespace BookRentalManager.Application.Mappers;

internal sealed class BookCreatedForAuthorDtoMapper : IMapper<Book, BookCreatedForAuthorDto>
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
