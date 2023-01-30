using BookRentalManager.Application.Books.Commands;
using BookRentalManager.Application.Books.CommandHandlers;

namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class CreateBookForAuthorCommandHandlerTests
{
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IMapper<Book, BookCreatedForAuthorDto>> _bookCreatedForAuthorDtoMapperStub;
    private readonly BookCreatedForAuthorDto _bookCreatedForAuthorDto;
    private readonly Author _author;
    private readonly CreateBookForAuthorCommand _createBookForAuthorCommand;
    private readonly CreateBookForAuthorCommandHandler _createBookForAuthorCommandHandler;

    public CreateBookForAuthorCommandHandlerTests()
    {
        Book book = TestFixtures.CreateDummyBook();
        _authorRepositoryStub = new();
        _bookRepositoryStub = new();
        _bookCreatedForAuthorDtoMapperStub = new();
        _bookCreatedForAuthorDto = new(book.Id, book.BookTitle, book.Edition.EditionNumber, book.Isbn.IsbnValue);
        _author = TestFixtures.CreateDummyAuthor();
        _createBookForAuthorCommand = new(_author.Id, book.BookTitle, book.Edition.EditionNumber, book.Isbn.IsbnValue);
        _createBookForAuthorCommandHandler = new(
            _bookRepositoryStub.Object,
            _authorRepositoryStub.Object,
            _bookCreatedForAuthorDtoMapperStub.Object);
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
        _bookCreatedForAuthorDtoMapperStub
            .Setup(bookCreatedForAuthorDtoMapper => bookCreatedForAuthorDtoMapper.Map(It.IsAny<Book>()))
            .Returns(_bookCreatedForAuthorDto);
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
        Result handleResult = await _createBookForAuthorCommandHandler.HandleAsync(_createBookForAuthorCommand, default);

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
        Result handleResult = await _createBookForAuthorCommandHandler.HandleAsync(_createBookForAuthorCommand, default);

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
        Result handleResult = await _createBookForAuthorCommandHandler.HandleAsync(_createBookForAuthorCommand, default);

        // Assert:
        Assert.True(handleResult.IsSuccess);
    }
}
