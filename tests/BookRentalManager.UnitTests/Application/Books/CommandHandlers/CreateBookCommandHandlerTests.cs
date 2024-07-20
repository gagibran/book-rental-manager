namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class CreateBookCommandHandlerTests
{
    private readonly Book _book;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly IReadOnlyList<Author> _authors;
    private readonly CreateBookCommand _createBookCommand;
    private readonly CreateBookCommandHandler _createBookCommandHandler;

    public CreateBookCommandHandlerTests()
    {
        _book = TestFixtures.CreateDummyBook();
        _authorRepositoryStub = new();
        _bookRepositoryStub = new();
        _authors = new List<Author>
        {
            TestFixtures.CreateDummyAuthor(),
            new(FullName.Create("Sarah", "Smith").Value!)
        }.AsReadOnly();
        _createBookCommand = new(
            _authors.Select(author => author.Id),
            _book.BookTitle.Title,
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
        // Act:
        Result handleAsyncResult = await _createBookCommandHandler.HandleAsync(
            new CreateBookCommand(
                [],
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<string>()),
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal("missingAuthorIds", handleAsyncResult.ErrorType);
        Assert.Equal("'authorIds' is a required field", handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingAuthor_ReturnsErrorMessage()
    {
        // Arrange:
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
        Assert.Equal("authorIds", handleAsyncResult.ErrorType);
        Assert.Equal("Could not find some of the authors for the provided IDs.", handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithErrorOnCreatingBook_ReturnsErrorMessage()
    {
        // Act:
        Result handleAsyncResult = await _createBookCommandHandler.HandleAsync(
            new CreateBookCommand(
                [It.IsAny<Guid>(), It.IsAny<Guid>()],
                string.Empty,
                -2,
                string.Empty),
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal("bookTitle|editionNumber|isbnFormat", handleAsyncResult.ErrorType);
        Assert.Equal(
            "The title can't be empty.|The edition number can't be smaller than 1.|Invalid ISBN format.",
            handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithCorrectParameters_ReturnsSuccess()
    {
        // Act:
        Result handleAsyncResult = await _createBookCommandHandler.HandleAsync(
            _createBookCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.True(handleAsyncResult.IsSuccess);
    }
}
