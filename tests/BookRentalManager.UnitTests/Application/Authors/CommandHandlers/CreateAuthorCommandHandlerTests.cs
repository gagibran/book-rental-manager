namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class CreateAuthorCommandHandlerTests
{
    private readonly Author _author;
    private readonly CreateAuthorCommand _createAuthorCommand;
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Mock<IMapper<Author, AuthorCreatedDto>> _authorToAuthorCreatedDtoMapperStub;
    private readonly CreateAuthorCommandHandler _createAuthorCommandHandler;

    public CreateAuthorCommandHandlerTests()
    {
        _author = TestFixtures.CreateDummyAuthor();
        _createAuthorCommand = new("John", "Doe");
        _authorRepositoryStub = new();
        _authorToAuthorCreatedDtoMapperStub = new();
        _createAuthorCommandHandler = new(
            _authorRepositoryStub.Object,
            _authorToAuthorCreatedDtoMapperStub.Object);
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
        var expectedErrorMessage = $"An author named '{_author.FullName.ToString()}' already exists.";

        // Act:
        Result handleResult = await _createAuthorCommandHandler.HandleAsync(_createAuthorCommand, default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handleResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingAuthor_ReturnsSuccess()
    {
        // Arrange:
        var expectedAuthorCreatedDto = new AuthorCreatedDto(_author.Id, _author.FullName.FirstName, _author.FullName.LastName);
        _authorToAuthorCreatedDtoMapperStub
            .Setup(authorToAuthorCreatedDtoMapper => authorToAuthorCreatedDtoMapper.Map(It.IsAny<Author>()))
            .Returns(expectedAuthorCreatedDto);

        // Act:
        Result<AuthorCreatedDto> handleResult = await _createAuthorCommandHandler.HandleAsync(_createAuthorCommand, default);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedAuthorCreatedDto.Id, handleResult.Value!.Id);
        Assert.Equal(expectedAuthorCreatedDto.FirstName, handleResult.Value!.FirstName);
        Assert.Equal(expectedAuthorCreatedDto.LastName, handleResult.Value!.LastName);
    }
}
