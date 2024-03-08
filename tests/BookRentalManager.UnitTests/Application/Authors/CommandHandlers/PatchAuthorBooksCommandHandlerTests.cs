namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class PatchAuthorBooksCommandHandlerTests
{
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Author _author;
    private readonly Book _book;
    private readonly PatchAuthorBooksCommand _patchAuthorBooksCommand;
    private readonly PatchAuthorBooksCommandHandler _patchAuthorBooksCommandHandler;

    public PatchAuthorBooksCommandHandlerTests()
    {
        _book = TestFixtures.CreateDummyBook();
        var anotherBook = new Book(
            "Clean Code: A Handbook of Agile Software Craftsmanship",
            Edition.Create(1).Value!,
            Isbn.Create("978-0132350884").Value!);
        var bookIdsToAdd = new List<Guid> { _book.Id, anotherBook.Id };
        var operations = new List<Operation<PatchAuthorBooksDto>>
        {
            new("add", "/bookIds", It.IsAny<string>(), bookIdsToAdd)
        };
        var patchAuthorBooksDtoPatchDocument = new JsonPatchDocument<PatchAuthorBooksDto>(operations, new DefaultContractResolver());
        _author = TestFixtures.CreateDummyAuthor();
        _authorRepositoryStub = new();
        _bookRepositoryStub = new();
        _patchAuthorBooksCommandHandler = new(_authorRepositoryStub.Object, _bookRepositoryStub.Object);
        _patchAuthorBooksCommand = new(_author.Id, patchAuthorBooksDtoPatchDocument);
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<AuthorByIdWithBooksSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_author);
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetAllBySpecificationAsync(
                It.IsAny<BooksByIdsSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([_book, anotherBook]);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingAuthor_ReturnsErrorMessage()
    {
        // Arrange:
        var authorId = Guid.NewGuid();
        var expectedErrorMessage = $"No author with the ID of '{authorId}' was found.";
        var patchAuthorBooksCommand = new PatchAuthorBooksCommand(authorId, It.IsAny<JsonPatchDocument<PatchAuthorBooksDto>>());
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<AuthorByIdWithBooksSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        // Act:
        Result patchAuthorBooksCommandHandlerResult = await _patchAuthorBooksCommandHandler.HandleAsync(
            patchAuthorBooksCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, patchAuthorBooksCommandHandlerResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithAtLeastOneNonexistingBook_ReturnsErrorMessage()
    {
        // Arrange:
        var operations = new List<Operation<PatchAuthorBooksDto>>
        {
            new("add", "/bookIds", It.IsAny<string>(), new List<Guid> { Guid.NewGuid() })
        };
        var patchAuthorBooksDtoPatchDocument = new JsonPatchDocument<PatchAuthorBooksDto>(operations, new DefaultContractResolver());
        var patchAuthorBooksCommand = new PatchAuthorBooksCommand(It.IsAny<Guid>(), patchAuthorBooksDtoPatchDocument);
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
        Result patchAuthorBooksCommandHandlerResult = await _patchAuthorBooksCommandHandler.HandleAsync(
            patchAuthorBooksCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, patchAuthorBooksCommandHandlerResult.ErrorMessage);
    }

    [Theory]
    [InlineData("replace", "'replace' operation not allowed in this context.")]
    [InlineData("remove", "'remove' operation not allowed in this context.")]
    public async Task HandleAsync_WithUnsupportedOperations_ReturnsErrorMessage(string operation, string expectedErrorMessage)
    {
        // Arrange:
        var operations = new List<Operation<PatchAuthorBooksDto>>
        {
            new(operation, "/bookIds", It.IsAny<string>(), new List<Guid> { Guid.NewGuid() })
        };
        var patchAuthorBooksDtoPatchDocument = new JsonPatchDocument<PatchAuthorBooksDto>(operations, new DefaultContractResolver());
        var patchAuthorBooksCommand = new PatchAuthorBooksCommand(It.IsAny<Guid>(), patchAuthorBooksDtoPatchDocument);
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
        Result patchAuthorBooksCommandHandlerResult = await _patchAuthorBooksCommandHandler.HandleAsync(
            patchAuthorBooksCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, patchAuthorBooksCommandHandlerResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithSuccessfulParameters_ReturnsSuccess()
    {
        // Act:
        Result patchAuthorBooksCommandHandlerResult = await _patchAuthorBooksCommandHandler.HandleAsync(
            _patchAuthorBooksCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.True(patchAuthorBooksCommandHandlerResult.IsSuccess);
    }
}
