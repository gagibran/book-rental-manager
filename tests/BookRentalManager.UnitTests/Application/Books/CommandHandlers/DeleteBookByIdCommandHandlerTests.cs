namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class DeleteBookByIdCommandHandlerTests
{
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Book _book;
    private readonly DeleteBookByIdCommand _deleteBookByIdCommand;
    private readonly DeleteBookByIdCommandHandler _deleteBookByIdCommandHandler;

    public DeleteBookByIdCommandHandlerTests()
    {
        _bookRepositoryStub = new();
        _book = TestFixtures.CreateDummyBook();
        _deleteBookByIdCommand = new(_book.Id);
        _deleteBookByIdCommandHandler = new(_bookRepositoryStub.Object);
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
        Result handleAsyncResult = await _deleteBookByIdCommandHandler.HandleAsync(
            _deleteBookByIdCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithBookRentedByCustomer_ReturnsErrorMessage()
    {
        // Arrange:
        Customer customer = TestFixtures.CreateDummyCustomer();
        var expectedErrorMessage = $"This book is currently rented by {customer.FullName}. Return the book before deleting it.";
        customer.RentBook(_book);

        // Act:
        Result handleAsyncResult = await _deleteBookByIdCommandHandler.HandleAsync(
            _deleteBookByIdCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithExistingBookNotRented_ReturnsSuccess()
    {
        // Act:
        Result handleAsyncResult = await _deleteBookByIdCommandHandler.HandleAsync(
            _deleteBookByIdCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.True(handleAsyncResult.IsSuccess);
    }
}
