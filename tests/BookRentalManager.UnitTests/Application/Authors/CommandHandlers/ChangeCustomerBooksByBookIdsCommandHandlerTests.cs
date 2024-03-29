namespace BookRentalManager.UnitTests.Application.Books.CommandHandlers;

public sealed class ChangeCustomerBooksByBookIdsCommandHandlerTests
{
    private readonly Book _book;
    private readonly Book _anotherBook;
    private readonly Customer _customer;
    private readonly JsonPatchDocument<ChangeCustomerBooksByBookIdsDto> _changeCustomerBooksByBookIdsDtoDocument;
    private readonly ChangeCustomerBooksByBookIdsCommand _changeCustomerBooksByBookIdsCommand;
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly ChangeCustomerBooksByBookIdsCommandHandler _changeCustomerBooksByBookIdsCommandHandler;

    public ChangeCustomerBooksByBookIdsCommandHandlerTests()
    {
        _book = TestFixtures.CreateDummyBook();
        _anotherBook = new Book(
                "Clean Code: A Handbook of Agile Software Craftsmanship",
                Edition.Create(1).Value!,
                Isbn.Create("978-0132350884").Value!);
        var operations = new List<Operation<ChangeCustomerBooksByBookIdsDto>>
        {
            new Operation<ChangeCustomerBooksByBookIdsDto>("add", "/bookIds", It.IsAny<string>(), new List<Guid> { _book.Id, _anotherBook.Id })
        };
        _changeCustomerBooksByBookIdsDtoDocument = new(operations, new DefaultContractResolver());
        _customer = TestFixtures.CreateDummyCustomer();
        _changeCustomerBooksByBookIdsCommand = new(_customer.Id, _changeCustomerBooksByBookIdsDtoDocument, true);
        _customerRepositoryStub = new();
        _bookRepositoryStub = new();
        _changeCustomerBooksByBookIdsCommandHandler = new(_customerRepositoryStub.Object, _bookRepositoryStub.Object);
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetAllBySpecificationAsync(
                It.IsAny<BooksByIdsSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book> { _book, _anotherBook });
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<CustomerByIdWithBooksSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_customer);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingCustomer_ReturnsErrorMessage()
    {
        // Arrange:
        var id = Guid.NewGuid();
        var expectedErrorMessage = $"No customer with the ID of '{id}' was found.";
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<CustomerByIdWithBooksSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);
        var changeCustomerBooksByBookIdsCommand = new ChangeCustomerBooksByBookIdsCommand(
            id,
            It.IsAny<JsonPatchDocument<ChangeCustomerBooksByBookIdsDto>>(),
            It.IsAny<bool>());

        // Act:
        Result handleAsyncResult = await _changeCustomerBooksByBookIdsCommandHandler.HandleAsync(
            changeCustomerBooksByBookIdsCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Theory]
    [InlineData("replace")]
    [InlineData("remove")]
    public async Task HandleAsync_WithNoSupportedPatchOperation_ReturnsErrorMessage(string operation)
    {
        // Arrange:
        var expectedErrorMessage = $"'{operation}' operation not allowed in this context.";
        var operations = new List<Operation<ChangeCustomerBooksByBookIdsDto>>
        {
            new Operation<ChangeCustomerBooksByBookIdsDto>(operation, "/bookIds", It.IsAny<string>(), new List<Guid> { Guid.NewGuid() })
        };
        var changeCustomerBooksByBookIdsDtoDocument = new JsonPatchDocument<ChangeCustomerBooksByBookIdsDto>(operations, new DefaultContractResolver());
        var changeCustomerBooksByBookIdsCommand = new ChangeCustomerBooksByBookIdsCommand(
            _customer.Id,
            changeCustomerBooksByBookIdsDtoDocument,
            It.IsAny<bool>());

        // Act:
        Result handleAsyncResult = await _changeCustomerBooksByBookIdsCommandHandler.HandleAsync(
            changeCustomerBooksByBookIdsCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingBooks_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = "Could not find some of the books for the provided IDs.";
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetAllBySpecificationAsync(
                It.IsAny<BooksByIdsSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book>());

        // Act:
        Result handleAsyncResult = await _changeCustomerBooksByBookIdsCommandHandler.HandleAsync(
            _changeCustomerBooksByBookIdsCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingBooksForCustomer_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = $"The book '{_book.BookTitle}' does not exist for this customer.|The book '{_anotherBook.BookTitle}' does not exist for this customer.";

        // Act:
        Result handleAsyncResult = await _changeCustomerBooksByBookIdsCommandHandler.HandleAsync(
            _changeCustomerBooksByBookIdsCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithCorrectParametersAndReturningBook_ReturnsSuccess()
    {
        // Arrange:
        _customer.RentBook(_book);
        _customer.RentBook(_anotherBook);

        // Act:
        Result handleAsyncResult = await _changeCustomerBooksByBookIdsCommandHandler.HandleAsync(
            _changeCustomerBooksByBookIdsCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.True(handleAsyncResult.IsSuccess);
    }


    [Fact]
    public async Task HandleAsync_WithCorrectParametersAndRentingBook_ReturnsSuccess()
    {
        // Arrange:
        var changeCustomerBooksByBookIdsCommand = new ChangeCustomerBooksByBookIdsCommand(
            _customer.Id,
            _changeCustomerBooksByBookIdsDtoDocument,
            false);

        // Act:
        Result handleAsyncResult = await _changeCustomerBooksByBookIdsCommandHandler.HandleAsync(
            changeCustomerBooksByBookIdsCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.True(handleAsyncResult.IsSuccess);
    }
}
