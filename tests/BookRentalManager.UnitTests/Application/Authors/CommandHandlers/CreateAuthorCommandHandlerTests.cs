using BookRentalManager.Application.Authors.CommandHandlers;
using BookRentalManager.Application.Authors.Commands;

namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class CreateAuthorCommandHandlerTests
{
    private readonly Author _author;
    private readonly CreateAuthorCommand _createAuthorCommand;
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IMapper<Author, AuthorCreatedDto>> _authorToAuthorCreatedDtoMapperStub;
    private readonly CreateAuthorCommandHandler _createAuthorCommandHandler;
    private readonly List<CreateBookForAuthorDto> _createBookForAuthorDtos;
    private readonly Book _book;

    public CreateAuthorCommandHandlerTests()
    {
        _book = TestFixtures.CreateDummyBook();
        var createBookForAuthorDto = new CreateBookForAuthorDto(_book.BookTitle, _book.Edition.EditionNumber, _book.Isbn.IsbnValue);
        _createBookForAuthorDtos = new List<CreateBookForAuthorDto> { createBookForAuthorDto };
        _author = TestFixtures.CreateDummyAuthor();
        _createAuthorCommand = new("John", "Doe", _createBookForAuthorDtos.AsReadOnly());
        _authorRepositoryStub = new();
        _bookRepositoryStub = new();
        _authorToAuthorCreatedDtoMapperStub = new();
        _createAuthorCommandHandler = new(
            _authorRepositoryStub.Object,
            _bookRepositoryStub.Object,
            _authorToAuthorCreatedDtoMapperStub.Object);
    }

    public static IEnumerable<object[]> GetIncorrectCreateBookForAuthorDtos()
    {
        yield return new object[]
        {
            new List<CreateBookForAuthorDto>
            {
                new CreateBookForAuthorDto("Les Misérables", 1, "97843-0140444308")
            },
            "Invalid ISBN format."
        };
        yield return new object[]
        {
            new List<CreateBookForAuthorDto>
            {
                new CreateBookForAuthorDto("Les Misérables", 0, "978-0140444308")
            },
            "The edition number can't be smaller than 1."
        };
        yield return new object[]
        {
            new List<CreateBookForAuthorDto>
            {
                new CreateBookForAuthorDto("Les Misérables", 0, "97843-0140444308")
            },
            "The edition number can't be smaller than 1.|Invalid ISBN format."
        };
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
        var expectedErrorMessage = $"An author named '{_author.FullName.CompleteName}' already exists.";

        // Act:
        Result handleResult = await _createAuthorCommandHandler.HandleAsync(_createAuthorCommand, default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handleResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithExistingBook_ReturnsErrorMessage()
    {
        // Arrange:
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<BooksByIsbnsSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestFixtures.CreateDummyBook());
        var expectedErrorMessage = "A book with the ISBN '0-201-61622-X' already exists.";

        // Act:
        Result handleResult = await _createAuthorCommandHandler.HandleAsync(_createAuthorCommand, default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handleResult.ErrorMessage);
    }

    [Theory]
    [MemberData(nameof(GetIncorrectCreateBookForAuthorDtos))]
    public async Task HandleAsync_WithIncorrectBookData_ReturnsErrorMessage(
        List<CreateBookForAuthorDto> createBookForAuthorDtos,
        string expectedErrorMessage)
    {
        // Arrange:
        var createAuthorCommandWithIncorrectBookData = new CreateAuthorCommand("John", "Doe", createBookForAuthorDtos.AsReadOnly());

        // Act:
        Result handleResult = await _createAuthorCommandHandler.HandleAsync(createAuthorCommandWithIncorrectBookData, default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handleResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithDuplicateBook_ReturnsErrorMessage()
    {
        // Arrange:
        _createBookForAuthorDtos.Add(new CreateBookForAuthorDto(
            _book.BookTitle,
            _book.Edition.EditionNumber,
            _book.Isbn.IsbnValue));
        var createAuthorCommand = new CreateAuthorCommand("John", "Doe", _createBookForAuthorDtos);
        var expectedErrorMessage = $"A book with the ISBN '{_book.Isbn.IsbnValue}' has already been added to this author.";

        // Act:
        Result handleResult = await _createAuthorCommandHandler.HandleAsync(createAuthorCommand, default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handleResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithCorrectBookDataAndNonexistingAuthor_ReturnsSuccess()
    {
        // Act:
        Result handleResult = await _createAuthorCommandHandler.HandleAsync(_createAuthorCommand, default);

        // Assert:
        Assert.True(handleResult.IsSuccess);
    }
}
