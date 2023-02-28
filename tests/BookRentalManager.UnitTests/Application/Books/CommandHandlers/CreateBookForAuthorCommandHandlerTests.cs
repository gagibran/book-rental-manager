using BookRentalManager.Application.Books.Commands;
using BookRentalManager.Application.Books.CommandHandlers;

namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class CreateBookForAuthorCommandHandlerTests
{
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IMapper<Book, BookCreatedForAuthorDto>> _bookToBookCreatedForAuthorDtoMapperStub;
    private readonly BookCreatedForAuthorDto _bookCreatedForAuthorDto;
    private readonly Book _book;
    private readonly Author _author;
    private readonly CreateBookForAuthorCommand _createBookForAuthorCommand;
    private readonly CreateBookForAuthorCommandHandler _createBookForAuthorCommandHandler;

    public CreateBookForAuthorCommandHandlerTests()
    {
        _book = TestFixtures.CreateDummyBook();
        _authorRepositoryStub = new();
        _bookRepositoryStub = new();
        _bookToBookCreatedForAuthorDtoMapperStub = new();
        _bookCreatedForAuthorDto = new(_book.Id, _book.BookTitle, _book.Edition.EditionNumber, _book.Isbn.IsbnValue);
        _author = TestFixtures.CreateDummyAuthor();
        _createBookForAuthorCommand = new(_author.Id, _book.BookTitle, _book.Edition.EditionNumber, _book.Isbn.IsbnValue);
        _createBookForAuthorCommandHandler = new(
            _bookRepositoryStub.Object,
            _authorRepositoryStub.Object,
            _bookToBookCreatedForAuthorDtoMapperStub.Object);
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<AuthorByIdWithBooksSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_author);
        _bookToBookCreatedForAuthorDtoMapperStub
            .Setup(bookToBookCreatedForAuthorDtoMapper => bookToBookCreatedForAuthorDtoMapper.Map(It.IsAny<Book>()))
            .Returns(_bookCreatedForAuthorDto);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingAuthor_ReturnsErrorMessage()
    {
        // Arrange:
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<AuthorByIdWithBooksSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author)null!);
        var expectedErrorMessage = $"No author with the ID of '{_author.Id}' was found.";

        // Act:
        Result handleResult = await _createBookForAuthorCommandHandler.HandleAsync(_createBookForAuthorCommand, default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handleResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingBookTitle_ReturnsSuccess()
    {
        // Act:
        Result handleResult = await _createBookForAuthorCommandHandler.HandleAsync(_createBookForAuthorCommand, default);

        // Assert:
        Assert.True(handleResult.IsSuccess);
    }
}
