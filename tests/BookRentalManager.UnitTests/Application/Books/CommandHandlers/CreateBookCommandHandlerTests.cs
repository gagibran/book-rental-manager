using BookRentalManager.Application.Books.Commands;
using BookRentalManager.Application.Books.CommandHandlers;

namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class CreateBookCommandHandlerTests
{
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IMapper<Book, BookCreatedDto>> _bookCreatedDtoMapperStub;
    private readonly BookCreatedDto _bookCreatedDto;
    private readonly Author _author;
    private readonly CreateBookCommand _createBookCommand;
    private readonly CreateBookCommandHandler _createBookCommandHandler;

    public CreateBookCommandHandlerTests()
    {
        Book book = TestFixtures.CreateDummyBook();
        _authorRepositoryStub = new();
        _bookRepositoryStub = new();
        _bookCreatedDtoMapperStub = new();
        _bookCreatedDto = new(book.Id, book.BookTitle, book.Edition.EditionNumber, book.Isbn.IsbnValue);
        _author = TestFixtures.CreateDummyAuthor();
        _createBookCommand = new(_author.Id, book.BookTitle, book.Edition.EditionNumber, book.Isbn.IsbnValue);
        _createBookCommandHandler = new(
            _bookRepositoryStub.Object,
            _authorRepositoryStub.Object,
            _bookCreatedDtoMapperStub.Object);
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Author>>(),
                default))
            .ReturnsAsync(_author);
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
    public async Task HandleAsync_WithNonexistingAuthor_ReturnsErrorMessage()
    {
        // Arrange:
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Author>>(),
                default))
            .ReturnsAsync((Author)null);
        var expectedErrorMessage = $"No author with the ID of '{_author.Id}' was found.";

        // Act:
        Result handleResult = await _createBookCommandHandler.HandleAsync(_createBookCommand, default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handleResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithExistingBookTitle_ReturnsErrorMessage()
    {
        // Arrange:
        _author.AddBook(TestFixtures.CreateDummyBook());
        var expectedErrorMessage = "A book with the ISBN '0-201-61622-X' has already been added to this author.";

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
