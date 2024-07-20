namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class PatchBookTitleEditionAndIsbnByIdCommandHandlerTests
{
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Book _book;
    private readonly PatchBookTitleEditionAndIsbnByIdCommand _patchBookTitleEditionAndIsbnByIdCommand;
    private readonly PatchBookTitleEditionAndIsbnByIdCommandHandler _patchBookTitleEditionAndIsbnByIdCommandHandler;

    public PatchBookTitleEditionAndIsbnByIdCommandHandlerTests()
    {
        _bookRepositoryStub = new();
        _book = TestFixtures.CreateDummyBook();
        var patchBookTitleEditionAndIsbnByIdDto = new PatchBookTitleEditionAndIsbnByIdDto(
            _book.BookTitle.Title,
            _book.Edition.EditionNumber,
            _book.Isbn.IsbnValue);
        var operations = new List<Operation<PatchBookTitleEditionAndIsbnByIdDto>>
        {
            new("replace", "/edition", It.IsAny<string>(), 2)
        };
        var patchBookTitleEditionAndIsbnByIdDtoPatchDocument = new JsonPatchDocument<PatchBookTitleEditionAndIsbnByIdDto>(
            operations,
            new DefaultContractResolver());
        _patchBookTitleEditionAndIsbnByIdCommand = new(_book.Id, patchBookTitleEditionAndIsbnByIdDtoPatchDocument);
        _patchBookTitleEditionAndIsbnByIdCommandHandler = new(_bookRepositoryStub.Object);
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<BookByIdWithAuthorsAndCustomersSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_book);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingBook_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = $"No book with the ID of '{_book.Id}' was found.";
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<BookByIdWithAuthorsAndCustomersSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        // Act:
        Result handleAsyncResult = await _patchBookTitleEditionAndIsbnByIdCommandHandler.HandleAsync(
            _patchBookTitleEditionAndIsbnByIdCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal("idNotFound", handleAsyncResult.ErrorType);
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Theory]
    [InlineData("/invalidPath", "/invalidPath", 2, "0-201-61622-X", "The target location specified by path segment 'invalidPath' was not found.", "jsonPatch")]
    [InlineData("/edition", "/isbn", 0, "0-201-61622-X", "The edition number can't be smaller than 1.", "editionNumber")]
    [InlineData("/edition", "/isbn", 1, "201-61622-X", "Invalid ISBN format.", "isbnFormat")]
    public async Task HandleAsync_WithInvalidPatchOperationOrValue_ReturnsErrorMessage(
        string path1,
        string path2,
        int newEdition,
        string newIsbn,
        string expectedErrorMessage,
        string expectedErrorType)
    {
        // Arrange:
        var operations = new List<Operation<PatchBookTitleEditionAndIsbnByIdDto>>
        {
            new("replace", path1, It.IsAny<string>(), newEdition),
            new("replace", path2, It.IsAny<string>(), newIsbn)
        };
        var patchBookTitleEditionAndIsbnByIdDtoPatchDocument = new JsonPatchDocument<PatchBookTitleEditionAndIsbnByIdDto>(
            operations,
            new DefaultContractResolver());

        // Act:
        Result handleAsyncResult = await _patchBookTitleEditionAndIsbnByIdCommandHandler.HandleAsync(
            new PatchBookTitleEditionAndIsbnByIdCommand(_book.Id, patchBookTitleEditionAndIsbnByIdDtoPatchDocument),
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorType, handleAsyncResult.ErrorType);
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithBookAlreadyRentedByCustomer_ReturnsErrorMessage()
    {
        // Arrange:
        Customer customer = TestFixtures.CreateDummyCustomer();
        var expectedErrorMessage = $"This book is currently rented by {customer.FullName}. Return the book before updating it.";
        _book.SetRentedBy(customer);

        // Act:
        Result handleAsyncResult = await _patchBookTitleEditionAndIsbnByIdCommandHandler.HandleAsync(
            _patchBookTitleEditionAndIsbnByIdCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal("bookCustomer", handleAsyncResult.ErrorType);
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithValidParameters_ReturnsSuccess()
    {
        // Act:
        Result handleAsyncResult = await _patchBookTitleEditionAndIsbnByIdCommandHandler.HandleAsync(
            _patchBookTitleEditionAndIsbnByIdCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.True(handleAsyncResult.IsSuccess);
    }
}
