namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class UpdateAuthorBooksCommandHandlerTests
{
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Author _author;
    private readonly Book _book;
    private readonly IReadOnlyList<Book> _booksToAdd;
    private readonly UpdateAuthorBooksCommand _updateAuthorBooksCommand;
    private readonly UpdateAuthorBooksCommandHandler _updateAuthorBooksCommandHandler;

    public UpdateAuthorBooksCommandHandlerTests()
    {
        var anotherBook = new Book(
            "Clean Code: A Handbook of Agile Software Craftsmanship",
            Edition.Create(1).Value!,
            Isbn.Create("978-0132350884").Value!);
        _author = TestFixtures.CreateDummyAuthor();
        _authorRepositoryStub = new();
        _bookRepositoryStub = new();
        _book = TestFixtures.CreateDummyBook();
        _booksToAdd = new List<Book> { _book, anotherBook }.AsReadOnly();
        _updateAuthorBooksCommandHandler = new(_authorRepositoryStub.Object, _bookRepositoryStub.Object);
        _updateAuthorBooksCommand = new(
            _author.Id,
            _booksToAdd.Select(bookToAdd => bookToAdd.Id).ToList().AsReadOnly());
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<AuthorByIdWithBooksSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_author);
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetAllBySpecificationAsync(
                It.IsAny<BooksByIdsSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_booksToAdd);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingAuthor_ReturnsErrorMessage()
    {
        // Arrange:
        var authorId = Guid.NewGuid();
        var expectedErrorMessage = $"No author with the ID of '{authorId}' was found.";
        var updateAuthorBooksCommand = new UpdateAuthorBooksCommand(authorId, It.IsAny<IReadOnlyList<Guid>>());
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<AuthorByIdWithBooksSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        // Act:
        Result updateAuthorBooksCommandHandlerResult = await _updateAuthorBooksCommandHandler.HandleAsync(
            updateAuthorBooksCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, updateAuthorBooksCommandHandlerResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithAtLeastOneNonexistingBook_ReturnsErrorMessage()
    {
        // Arrange:
        var bookIds = new List<Guid> { Guid.NewGuid() }.AsReadOnly();
        var updateAuthorBooksCommand = new UpdateAuthorBooksCommand(It.IsAny<Guid>(), bookIds);
        var expectedErrorMessage = "Could not find some of the books for the provided IDs.";
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<AuthorByIdWithBooksSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestFixtures.CreateDummyAuthor());
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetAllBySpecificationAsync(
                It.IsAny<BooksByIdsSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book>().AsReadOnly());

        // Act:
        Result updateAuthorBooksCommandHandlerResult = await _updateAuthorBooksCommandHandler.HandleAsync(
            updateAuthorBooksCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, updateAuthorBooksCommandHandlerResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithAuthorWithBookAlreadyRegistered_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = $"A book with the ISBN '{_book.Isbn.IsbnValue}' has already been added to this author.";
        _author.AddBook(_book);

        // Act:
        Result updateAuthorBooksCommandHandlerResult = await _updateAuthorBooksCommandHandler.HandleAsync(
            _updateAuthorBooksCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, updateAuthorBooksCommandHandlerResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithExistingAuthorAndBooksNotRegistered_ReturnsSuccess()
    {
        // Act:
        Result updateAuthorBooksCommandHandlerResult = await _updateAuthorBooksCommandHandler.HandleAsync(
            _updateAuthorBooksCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.True(updateAuthorBooksCommandHandlerResult.IsSuccess);
    }
}
