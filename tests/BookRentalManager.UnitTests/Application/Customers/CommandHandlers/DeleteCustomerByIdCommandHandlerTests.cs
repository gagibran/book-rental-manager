namespace BookRentalManager.UnitTests.Application.Customers.CommandHandlers;

public sealed class DeleteCustomerByIdCommandHandlerTests
{
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly DeleteCustomerByIdCommandHandler _deleteCustomerByIdCommandHandler;
    private readonly Customer _customer;
    private readonly DeleteCustomerByIdCommand _deleteCustomerByIdCommand;

    public DeleteCustomerByIdCommandHandlerTests()
    {
        _customer = TestFixtures.CreateDummyCustomer();
        _customerRepositoryStub = new();
        _deleteCustomerByIdCommand = new(_customer.Id);
        _deleteCustomerByIdCommandHandler = new(_customerRepositoryStub.Object);
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
        var expectedErrorMessage = $"No customer with the ID of '{_customer.Id}' was found.";
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<CustomerByIdWithBooksSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act:
        Result deleteCustomerByIdResult = await _deleteCustomerByIdCommandHandler.HandleAsync(
            _deleteCustomerByIdCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, deleteCustomerByIdResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithCustomerWithBooks_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = "This customer has rented books. Return them before deleting.";
        Book book = TestFixtures.CreateDummyBook();
        _customer.RentBook(book);

        // Act:
        Result deleteCustomerByIdResult = await _deleteCustomerByIdCommandHandler.HandleAsync(
            _deleteCustomerByIdCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, deleteCustomerByIdResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithCustomerWithoutBooks_ReturnsSuccess()
    {
        // Act:
        Result deleteCustomerByIdResult = await _deleteCustomerByIdCommandHandler.HandleAsync(
            _deleteCustomerByIdCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.True(deleteCustomerByIdResult.IsSuccess);
    }
}
