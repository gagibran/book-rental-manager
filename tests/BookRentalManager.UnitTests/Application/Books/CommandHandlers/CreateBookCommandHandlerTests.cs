namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class CreateBookCommandHandlerTests
{
    private readonly Book _book;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Mock<IMapper<Book, BookCreatedDto>> _bookToBookCreatedDtoMapperStub;
    private readonly BookCreatedDto _bookCreatedDto;
    private readonly IReadOnlyList<Author> _authors;
    private readonly CreateBookCommand _createBookCommand;
    private readonly CreateBookCommandHandler _createBookCommandHandler;

    public CreateBookCommandHandlerTests()
    {
        _book = TestFixtures.CreateDummyBook();
        _authorRepositoryStub = new();
        _bookRepositoryStub = new();
        _bookToBookCreatedDtoMapperStub = new();
        _bookCreatedDto = new(_book.Id, _book.BookTitle, _book.Edition.EditionNumber, _book.Isbn.IsbnValue);
        _authors = new List<Author>
        {
            TestFixtures.CreateDummyAuthor(),
            new Author(FullName.Create("Sarah", "Smith").Value!)
        }.AsReadOnly();
        _createBookCommand = new(
            _authors.Select(author => author.Id),
            _book.BookTitle,
            _book.Edition.EditionNumber,
            _book.Isbn.IsbnValue);
        _createBookCommandHandler = new(
            _bookRepositoryStub.Object,
            _authorRepositoryStub.Object,
            _bookToBookCreatedDtoMapperStub.Object);
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetAllBySpecificationAsync(
                It.IsAny<AuthorsByIdsSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_authors);
        _bookToBookCreatedDtoMapperStub
            .Setup(bookToBookCreatedDtoMapper => bookToBookCreatedDtoMapper.Map(It.IsAny<Book>()))
            .Returns(_bookCreatedDto);
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
    public async Task HandleAsync_WithNonExistingBookTitle_ReturnsSuccess()
    {
        // Act:
        Result handleAsyncResult = await _createBookCommandHandler.HandleAsync(
            _createBookCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.True(handleAsyncResult.IsSuccess);
    }
}
