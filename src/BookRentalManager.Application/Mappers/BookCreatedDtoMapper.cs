namespace BookRentalManager.Application.Mappers;

internal sealed class BookForAuthorCreatedDtoMapper : IMapper<Book, BookForAuthorCreatedDto>
{
    public BookForAuthorCreatedDto Map(Book book)
    {
        return new BookForAuthorCreatedDto(
            book.Id,
            book.BookTitle,
            book.Edition.EditionNumber,
            book.Isbn.IsbnValue);
    }
}
