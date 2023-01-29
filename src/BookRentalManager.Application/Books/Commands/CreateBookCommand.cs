namespace BookRentalManager.Application.Books.Commands;

public sealed class CreateBookCommand : ICommand<BookCreatedDto>
{
    public Guid BookAuthorId { get; }
    public CreateBookDto CreateBookDto { get; }

    public CreateBookCommand(Guid bookAuthorId, CreateBookDto createBookDto)
    {
        BookAuthorId = bookAuthorId;
        CreateBookDto = createBookDto;
    }
}
