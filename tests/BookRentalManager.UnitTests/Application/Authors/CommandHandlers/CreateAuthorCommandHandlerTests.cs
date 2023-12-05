namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class CreateAuthorCommandHandlerTests
{
    private readonly Author _author;
    private readonly CreateAuthorCommand _createAuthorCommand;
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly CreateAuthorCommandHandler _createAuthorCommandHandler;

    public CreateAuthorCommandHandlerTests()
    {
        _author = TestFixtures.CreateDummyAuthor();
        _createAuthorCommand = new("John", "Doe");
        _authorRepositoryStub = new();
        _createAuthorCommandHandler = new(_authorRepositoryStub.Object);
    }

    [Fact]
    public async Task HandleAsync_WithExistingAuthor_ReturnsErrorMessage()
    {
        // Arrange:
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Author>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_author);
        var expectedErrorMessage = $"An author named '{_author.FullName}' already exists.";

        // Act:
        Result handleResult = await _createAuthorCommandHandler.HandleAsync(_createAuthorCommand, It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handleResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingAuthor_ReturnsSuccess()
    {
        // Arrange:
        var expectedAuthorCreatedDto = new AuthorCreatedDto(It.IsAny<Guid>(), _author.FullName.FirstName, _author.FullName.LastName);

        // Act:
        Result<AuthorCreatedDto> handleResult = await _createAuthorCommandHandler.HandleAsync(
            _createAuthorCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.IsType<Guid>(handleResult.Value!.Id);
        Assert.Equal(expectedAuthorCreatedDto.FirstName, handleResult.Value!.FirstName);
        Assert.Equal(expectedAuthorCreatedDto.LastName, handleResult.Value!.LastName);
    }
}
