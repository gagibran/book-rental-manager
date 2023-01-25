using BookRentalManager.Application.Books.Commands;
using BookRentalManager.Application.Books.CommandHandlers;

namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class CreateBookCommandHandlerTests
{
    private readonly Mock<IRepository<BookAuthor>> _bookAuthorRepositoryStub;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Book _book;
    private readonly BookAuthor _bookAuthor;
    private readonly CreateBookCommand _createBookCommand;
    private readonly CreateBookCommandHandler _createBookCommandHandler;

    public CreateBookCommandHandlerTests()
    {
        _bookAuthorRepositoryStub = new();
        _bookRepositoryStub = new();
        _book = TestFixtures.CreateDummyBook();
        _bookAuthor = TestFixtures.CreateDummyBookAuthor();
        _createBookCommand = new(_bookAuthor.Id, _book);
        _createBookCommandHandler = new(_bookRepositoryStub.Object, _bookAuthorRepositoryStub.Object);
        _bookAuthorRepositoryStub
            .Setup(bookAuthorRepository => bookAuthorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<BookAuthor>>(),
                default))
            .ReturnsAsync(_bookAuthor);
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
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Book>>(),
                default))
            .ReturnsAsync(_book);
        var expectedErrorMessage = "A book with the ISBN '0-201-61622-X' already exists for this book author.";
        _bookRepositoryStub
            .Setup(bookRepository =>
                bookRepository.GetFirstOrDefaultBySpecificationAsync(
                    It.IsAny<Specification<Book>>(),
                    default))
            .ReturnsAsync(_book);

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
