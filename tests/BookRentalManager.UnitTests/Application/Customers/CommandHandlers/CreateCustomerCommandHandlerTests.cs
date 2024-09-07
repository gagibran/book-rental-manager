namespace BookRentalManager.UnitTests.Application.Customers.CommandHandlers;

public sealed class CreateCustomerCommandHandlerTests
{
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly CreateCustomerCommand _createCustomerCommand;
    private readonly CreateCustomerCommandHandler _createCustomerCommandHandler;

    public CreateCustomerCommandHandlerTests()
    {
        _customerRepositoryStub = new();
        _createCustomerCommand = new("John", "Doe", "john.doe@email.com", new PhoneNumberDto(200, 2_000_000));
        _createCustomerCommandHandler = new(_customerRepositoryStub.Object);
    }

    [Fact]
    public async Task HandleAsync_WithExistingCustomerEmail_ReturnsErrorMessage()
    {
        // Act:
        Customer customer = TestFixtures.CreateDummyCustomer();
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Customer>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        Result handleResult = await _createCustomerCommandHandler.HandleAsync(_createCustomerCommand, It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal("customerEmailAlreadyExists", handleResult.ErrorType);
        Assert.Equal("A customer with the email 'john.doe@email.com' already exists.", handleResult.ErrorMessage);
    }

    [Theory]
    [InlineData("", "Doe", "john.doe@email.com", 200, 2_000_000, "firstName", "First name cannot be empty.")]
    [InlineData("John", "  ", "john.doe@email.com", 200, 2_000_000, "lastName", "Last name cannot be empty.")]
    [InlineData("John", "Doe", "", 200, 2_000_000, "email", "Email address is not in a valid format.")]
    [InlineData("John", "Doe", "john.doe@email.com", -2, 2_000_000, "invalidAreaCode", "Invalid area code.")]
    [InlineData("John", "Doe", "john.doe@email.com", 200, 3, "invalidPhoneNumber", "Invalid phone number.")]
    [InlineData("John", "Doe", "", 1, 2_000_000, "email|invalidAreaCode", "Email address is not in a valid format.|Invalid area code.")]
    [InlineData(
        "",
        "",
        "",
        200,
        2_000_000,
        "firstName|lastName|email",
        "First name cannot be empty.|Last name cannot be empty.|Email address is not in a valid format.")]
    [InlineData(
        "",
        "",
        "",
        -3,
        1,
        "firstName|lastName|email|invalidAreaCode|invalidPhoneNumber",
        "First name cannot be empty.|Last name cannot be empty.|Email address is not in a valid format.|Invalid area code.|Invalid phone number.")]
    public async Task HandleAsync_WithInvalidCustomerParameters_ReturnsErrorMessage(
        string firstName,
        string lastName,
        string email,
        int areaCode,
        int prefixAndLineNumber,
        string expectedErrorType,
        string expectedErrorMessage)
    {
        // Arrange:
        var createCustomerCommand = new CreateCustomerCommand(
            firstName,
            lastName,
            email,
            new PhoneNumberDto(areaCode, prefixAndLineNumber));
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Customer>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act:
        Result handleResult = await _createCustomerCommandHandler.HandleAsync(createCustomerCommand, It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorType, handleResult.ErrorType);
        Assert.Equal(expectedErrorMessage, handleResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingCustomerEmail_ReturnsSuccess()
    {
        // Arrange:
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Customer>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);
        _customerRepositoryStub
            .Setup(customerRepository =>customerRepository.CreateAsync(
                It.IsAny<Customer>(),
                It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act:
        Result handleResult = await _createCustomerCommandHandler.HandleAsync(_createCustomerCommand, It.IsAny<CancellationToken>());

        // Assert:
        Assert.True(handleResult.IsSuccess);
    }
}
