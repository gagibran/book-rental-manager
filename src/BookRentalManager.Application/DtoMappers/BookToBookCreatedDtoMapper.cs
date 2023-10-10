namespace BookRentalManager.Application.DtoMappers;

internal sealed class BookToBookCreatedDtoMapper : IMapper<Book, BookCreatedDto>
{
    public BookCreatedDto Map(Book book)
    {
        return new BookCreatedDto(
            book.Id,
            book.BookTitle,
            book.Edition.EditionNumber,
            book.Isbn.IsbnValue);
    }
}
