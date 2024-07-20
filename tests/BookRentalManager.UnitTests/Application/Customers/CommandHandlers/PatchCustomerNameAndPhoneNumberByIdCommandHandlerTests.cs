namespace BookRentalManager.UnitTests.Application.Customers.CommandHandlers;

public sealed class PatchCustomerNameAndPhoneNumberByIdCommandHandlerTests
{
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly Customer _customer;
    private readonly PatchCustomerNameAndPhoneNumberByIdCommand _patchCustomerNameAndPhoneNumberByIdCommand;
    private readonly PatchCustomerNameAndPhoneNumberByIdCommandHandler _patchCustomerNameAndPhoneNumberByIdCommandHandler;

    public PatchCustomerNameAndPhoneNumberByIdCommandHandlerTests()
    {
        _customerRepositoryStub = new();
        _customer = TestFixtures.CreateDummyCustomer();
        var operations = new List<Operation<PatchCustomerNameAndPhoneNumberDto>>
        {
            new("replace", "/areaCode", It.IsAny<string>(), 222)
        };
        var patchCustomerNameAndPhoneNumberDtoJsonPatchDocument = new JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto>(
            operations,
            new DefaultContractResolver());
        var patchCustomerNameAndPhoneNumberDto = new PatchCustomerNameAndPhoneNumberDto(
            _customer.FullName.FirstName,
            _customer.FullName.LastName,
            _customer.PhoneNumber.AreaCode,
            _customer.PhoneNumber.PrefixAndLineNumber);
        _patchCustomerNameAndPhoneNumberByIdCommand = new(_customer.Id, patchCustomerNameAndPhoneNumberDtoJsonPatchDocument);
        _patchCustomerNameAndPhoneNumberByIdCommandHandler = new(_customerRepositoryStub.Object);
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
        Result handleAsyncResult = await _patchCustomerNameAndPhoneNumberByIdCommandHandler.HandleAsync(
            _patchCustomerNameAndPhoneNumberByIdCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal("idNotFound", handleAsyncResult.ErrorType);
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Theory]
    [InlineData("/invalidPath", "/invalidPath", "John", 222, "jsonPatch", "The target location specified by path segment 'invalidPath' was not found.")]
    [InlineData("/firstName", "/areaCode", "John", 100, "invalidAreaCode", "Invalid area code.")]
    [InlineData("/firstName", "/areaCode", " ", 300, "firstName", "First name cannot be empty.")]
    [InlineData("/firstName", "/areaCode", " ", 100, "firstName|invalidAreaCode", "First name cannot be empty.|Invalid area code.")]
    public async Task HandleAsync_WithInvalidPatchOperationOrValue_ReturnsErrorMessage(
        string path1,
        string path2,
        string firstName,
        int newAreaCode,
        string expectedErrorType,
        string expectedErrorMessage)
    {
        // Arrange:
        var operations = new List<Operation<PatchCustomerNameAndPhoneNumberDto>>
        {
            new("replace", path1, It.IsAny<string>(), firstName),
            new("replace", path2, It.IsAny<string>(), newAreaCode)
        };
        var patchCustomerNameAndPhoneNumberDtoJsonPatchDocument = new JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto>(
            operations,
            new DefaultContractResolver());

        // Act:
        Result handleAsyncResult = await _patchCustomerNameAndPhoneNumberByIdCommandHandler.HandleAsync(
            new PatchCustomerNameAndPhoneNumberByIdCommand(_customer.Id, patchCustomerNameAndPhoneNumberDtoJsonPatchDocument),
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorType, handleAsyncResult.ErrorType);
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithValidParameters_ReturnsSuccess()
    {
        // Act:
        Result handleAsyncResult = await _patchCustomerNameAndPhoneNumberByIdCommandHandler.HandleAsync(
            _patchCustomerNameAndPhoneNumberByIdCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.True(handleAsyncResult.IsSuccess);
    }
}
