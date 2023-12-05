namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class CreateBookCommandHandlerTests
{
    private readonly Book _book;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly BookCreatedDto _bookCreatedDto;
    private readonly IReadOnlyList<Author> _authors;
    private readonly CreateBookCommand _createBookCommand;
    private readonly CreateBookCommandHandler _createBookCommandHandler;

    public CreateBookCommandHandlerTests()
    {
        _book = TestFixtures.CreateDummyBook();
        _authorRepositoryStub = new();
        _bookRepositoryStub = new();
        _bookCreatedDto = new(_book.Id, _book.BookTitle, _book.Edition.EditionNumber, _book.Isbn.IsbnValue);
        _authors = new List<Author>
        {
            TestFixtures.CreateDummyAuthor(),
            new(FullName.Create("Sarah", "Smith").Value!)
        }.AsReadOnly();
        _createBookCommand = new(
            _authors.Select(author => author.Id),
            _book.BookTitle,
            _book.Edition.EditionNumber,
            _book.Isbn.IsbnValue);
        _createBookCommandHandler = new(_bookRepositoryStub.Object, _authorRepositoryStub.Object);
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetAllBySpecificationAsync(
                It.IsAny<AuthorsByIdsSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_authors);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyAuthorIdsList_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = "'authorIds' is a required field";

        // Act:
        Result handleAsyncResult = await _createBookCommandHandler.HandleAsync(
            new CreateBookCommand(
                new List<Guid>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<string>()),
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingAuthor_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = "Could not find some of the authors for the provided IDs.";
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetAllBySpecificationAsync(
                It.IsAny<AuthorsByIdsSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Author>().AsReadOnly());

        // Act:
        Result handleAsyncResult = await _createBookCommandHandler.HandleAsync(
            _createBookCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithExistingBook_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = $"A book with the title: '{_book.BookTitle}', edition: '{_book.Edition.EditionNumber}' and ISBN: '{_book.Isbn}' already exists.";
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<BookByTitleEditionAndIsbnSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_book);

        // Act:
        Result handleAsyncResult = await _createBookCommandHandler.HandleAsync(
            _createBookCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingBookTitle_ReturnsSuccess()
    {
        // Act:
        Result handleAsyncResult = await _createBookCommandHandler.HandleAsync(
            _createBookCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.True(handleAsyncResult.IsSuccess);
    }
}
