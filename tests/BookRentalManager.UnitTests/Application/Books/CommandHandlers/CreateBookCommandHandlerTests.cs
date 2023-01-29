using BookRentalManager.Application.Books.Commands;
using BookRentalManager.Application.Books.CommandHandlers;

namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class CreateBookCommandHandlerTests
{
    private readonly Mock<IRepository<BookAuthor>> _bookAuthorRepositoryStub;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IMapper<Book, BookCreatedDto>> _bookCreatedDtoMapperStub;
    private readonly BookCreatedDto _bookCreatedDto;
    private readonly BookAuthor _bookAuthor;
    private readonly CreateBookCommand _createBookCommand;
    private readonly CreateBookCommandHandler _createBookCommandHandler;

    public CreateBookCommandHandlerTests()
    {
        Book book = TestFixtures.CreateDummyBook();
        _bookAuthorRepositoryStub = new();
        _bookRepositoryStub = new();
        _bookCreatedDtoMapperStub = new();
        _bookCreatedDto = new(book.Id, book.BookTitle, book.Edition.EditionNumber, book.Isbn.IsbnValue);
        _bookAuthor = TestFixtures.CreateDummyBookAuthor();
        _createBookCommand = new(_bookAuthor.Id, book.BookTitle, book.Edition.EditionNumber, book.Isbn.IsbnValue);
        _createBookCommandHandler = new(
            _bookRepositoryStub.Object,
            _bookAuthorRepositoryStub.Object,
            _bookCreatedDtoMapperStub.Object);
        _bookAuthorRepositoryStub
            .Setup(bookAuthorRepository => bookAuthorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<BookAuthor>>(),
                default))
            .ReturnsAsync(_bookAuthor);
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Book>>(),
                default))
            .ReturnsAsync(book);
        _bookCreatedDtoMapperStub
            .Setup(bookCreatedDtoMapper => bookCreatedDtoMapper.Map(It.IsAny<Book>()))
            .Returns(_bookCreatedDto);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingBookAuthor_ReturnsErrorMessage()
    {
        // Arrange:
        _bookAuthorRepositoryStub
            .Setup(bookAuthorRepository => bookAuthorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<BookAuthor>>(),
                default))
            .ReturnsAsync((BookAuthor)null);
        var expectedErrorMessage = $"No book author with the ID of '{_bookAuthor.Id}' was found.";

        // Act:
        Result handleResult = await _createBookCommandHandler.HandleAsync(_createBookCommand, default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handleResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithExistingBookTitle_ReturnsErrorMessage()
    {
        // Arrange:
        _bookAuthor.AddBook(TestFixtures.CreateDummyBook());
        var expectedErrorMessage = "A book with the ISBN '0-201-61622-X' has already been added to this book author.";

        // Act:
        Result handleResult = await _createBookCommandHandler.HandleAsync(_createBookCommand, default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handleResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingBookTitle_ReturnsSuccess()
    {
        // Arrange:
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Book>>(),
                default))
            .ReturnsAsync((Book)null);
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.CreateAsync(It.IsAny<Book>(), default))
            .Verifiable();

        // Act:
        Result handleResult = await _createBookCommandHandler.HandleAsync(_createBookCommand, default);

        // Assert:
        Assert.True(handleResult.IsSuccess);
    }
}
