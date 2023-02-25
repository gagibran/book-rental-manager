namespace BookRentalManager.UnitTests.Application.Customers.CommandHandlers;

public sealed class PatchCustomerNameAndPhoneNumberCommandHandlerTests
{
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly Mock<IMapper<Customer, PatchCustomerNameAndPhoneNumberDto>> _customerToPatchCustomerNameAndPhoneNumberDtoMapperStub;
    private readonly Customer _customer;
    private readonly PatchCustomerNameAndPhoneNumberCommand _patchCustomerNameAndPhoneNumberCommand;
    private readonly PatchCustomerNameAndPhoneNumberCommandHandler _patchCustomerNameAndPhoneNumberCommandHandler;

    public PatchCustomerNameAndPhoneNumberCommandHandlerTests()
    {
        _customerRepositoryStub = new();
        _customerToPatchCustomerNameAndPhoneNumberDtoMapperStub = new();
        _customer = TestFixtures.CreateDummyCustomer();
        var operations = new List<Operation<PatchCustomerNameAndPhoneNumberDto>>
        {
            new Operation<PatchCustomerNameAndPhoneNumberDto>("replace", "/areaCode", It.IsAny<string>(), 222)
        };
        var patchCustomerNameAndPhoneNumberDtoJsonPatchDocument = new JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto>(
            operations,
            new DefaultContractResolver());
        var patchCustomerNameAndPhoneNumberDto = new PatchCustomerNameAndPhoneNumberDto(
            _customer.FullName.FirstName,
            _customer.FullName.LastName,
            _customer.PhoneNumber.AreaCode,
            _customer.PhoneNumber.PrefixAndLineNumber);
        _patchCustomerNameAndPhoneNumberCommand = new(_customer.Id, patchCustomerNameAndPhoneNumberDtoJsonPatchDocument);
        _patchCustomerNameAndPhoneNumberCommandHandler = new(
            _customerRepositoryStub.Object,
            _customerToPatchCustomerNameAndPhoneNumberDtoMapperStub.Object);
        _customerToPatchCustomerNameAndPhoneNumberDtoMapperStub
            .Setup(customerToPatchCustomerNameAndPhoneNumberDtoMapper => customerToPatchCustomerNameAndPhoneNumberDtoMapper.Map(It.IsAny<Customer>()))
            .Returns(patchCustomerNameAndPhoneNumberDto);
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
        Result handleAsyncResult = await _patchCustomerNameAndPhoneNumberCommandHandler.HandleAsync(
            _patchCustomerNameAndPhoneNumberCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Theory]
    [InlineData("/invalidPath", 222, "The target location specified by path segment 'invalidPath' was not found.")]
    [InlineData("/areaCode", 100, "Invalid area code.")]
    public async Task HandleAsync_WithInvalidPatchOperationOrValue_ReturnsErrorMessage(string path, int newAreaCode, string expectedErrorMessage)
    {
        // Arrange:
        var operations = new List<Operation<PatchCustomerNameAndPhoneNumberDto>>
        {
            new Operation<PatchCustomerNameAndPhoneNumberDto>("replace", path, It.IsAny<string>(), newAreaCode)
        };
        var patchCustomerNameAndPhoneNumberDtoJsonPatchDocument = new JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto>(
            operations,
            new DefaultContractResolver());

        // Act:
        Result handleAsyncResult = await _patchCustomerNameAndPhoneNumberCommandHandler.HandleAsync(
            new PatchCustomerNameAndPhoneNumberCommand(_customer.Id, patchCustomerNameAndPhoneNumberDtoJsonPatchDocument),
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Theory]
    [InlineData("", "Doe", 200, 2000000, "First name cannot be empty.")]
    [InlineData("John", "", 200, 2000000, "Last name cannot be empty.")]
    [InlineData("John", "Doe", 200, 99999991, "Invalid phone number.")]
    public async Task HandleAsync_WithInvalidNameAndPhoneNumber_ReturnsErrorMessage(
        string firstName,
        string lastName,
        int areaCode,
        int prefixAndLineNumber,
        string expectedErrorMessage)
    {
        // Arrange:
        var patchCustomerNameAndPhoneNumberDto = new PatchCustomerNameAndPhoneNumberDto(firstName, lastName, areaCode, prefixAndLineNumber);
        _customerToPatchCustomerNameAndPhoneNumberDtoMapperStub
            .Setup(customerToPatchCustomerNameAndPhoneNumberDtoMapper => customerToPatchCustomerNameAndPhoneNumberDtoMapper.Map(It.IsAny<Customer>()))
            .Returns(patchCustomerNameAndPhoneNumberDto);

        // Act:
        Result handleAsyncResult = await _patchCustomerNameAndPhoneNumberCommandHandler.HandleAsync(
            _patchCustomerNameAndPhoneNumberCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handleAsyncResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithValidParameters_ReturnsSuccess()
    {
        // Act:
        Result handleAsyncResult = await _patchCustomerNameAndPhoneNumberCommandHandler.HandleAsync(
            _patchCustomerNameAndPhoneNumberCommand,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.True(handleAsyncResult.IsSuccess);
    }
}
