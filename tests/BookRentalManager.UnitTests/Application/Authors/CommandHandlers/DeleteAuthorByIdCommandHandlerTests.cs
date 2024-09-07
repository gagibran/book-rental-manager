namespace BookRentalManager.UnitTests.Application.Authors.CommandHandlers;

public sealed class DeleteAuthorByIdCommandHandlerTests
{
    private readonly Author _author;
    private readonly DeleteAuthorByIdCommand _deleteAuthorByIdCommand;
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly DeleteAuthorByIdCommandHandler _deleteAuthorByIdCommandHandler;

    public DeleteAuthorByIdCommandHandlerTests()
    {
        _author = TestFixtures.CreateDummyAuthor();
        _deleteAuthorByIdCommand = new(_author.Id);
        _authorRepositoryStub = new();
        _deleteAuthorByIdCommandHandler = new(_authorRepositoryStub.Object);
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<AuthorByIdWithBooksSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_author);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingAuthor_ReturnsErrorMessage()
    {
        // Arrange:
        var id = Guid.NewGuid();
        var expectedErrorMessage = $"No author with the ID of '{id}' was found.";
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<AuthorByIdWithBooksSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);
        var deleteAuthorByIdCommand = new DeleteAuthorByIdCommand(id);

        // Act:
        Result handleAsyncResult = await _deleteAuthorByIdCommandHandler.HandleAsync(
            deleteAuthorByIdCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal("idNotFound", handleAsyncResult.ErrorType);
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithAuthorWithRentedBook_ReturnsErrorMessage()
    {
        // Arrange:
        Book book = TestFixtures.CreateDummyBook();
        _author.AddBook(book);

        // Act:
        Result handleAsyncResult = await _deleteAuthorByIdCommandHandler.HandleAsync(
            _deleteAuthorByIdCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal("authorHasBooks", handleAsyncResult.ErrorType);
        Assert.Equal("This author has books. Please, delete the books before deleting the author.", handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithValidParameters_ReturnsSuccess()
    {
        // Act:
        Result handleAsyncResult = await _deleteAuthorByIdCommandHandler.HandleAsync(
            _deleteAuthorByIdCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.True(handleAsyncResult.IsSuccess);
    }
}
