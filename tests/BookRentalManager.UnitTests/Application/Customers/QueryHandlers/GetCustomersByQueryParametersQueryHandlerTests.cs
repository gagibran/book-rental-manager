namespace BookRentalManager.UnitTests.Application.Customers.QueryHandlers;

public sealed class GetCustomersByQueryParametersQueryHandlerTests
{
    private readonly GetCustomersByQueryParametersQuery _getCustomersByQueryParametersQuery;
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly Mock<ISortParametersMapper> _sortParametersMapperStub;
    private readonly GetCustomersByQueryParametersQueryHandler _getCustomersByQueryParametersQueryHandler;

    public GetCustomersByQueryParametersQueryHandlerTests()
    {
        _getCustomersByQueryParametersQuery = new(
            It.IsAny<int>(),
            It.IsAny<int>(),
            string.Empty,
            It.IsAny<string>());
        _sortParametersMapperStub = new();
        _customerRepositoryStub = new();
        _getCustomersByQueryParametersQueryHandler = new(_customerRepositoryStub.Object, _sortParametersMapperStub.Object);
        _sortParametersMapperStub
            .Setup(sortParametersMapper => sortParametersMapper.MapCustomerSortParameters(It.IsAny<string>()))
            .Returns(Result.Success(string.Empty));
    }

    [Fact]
    public async Task HandleAsync_WithoutCustomers_ReturnsEmptyList()
    {
        // Arrange:
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Customer>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Customer>(
                [],
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()));

        // Act:
        Result<PaginatedList<GetCustomerDto>> handlerResult = await _getCustomersByQueryParametersQueryHandler.HandleAsync(
            _getCustomersByQueryParametersQuery,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Empty(handlerResult.Value!);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidSortParameters_ReturnsErrorMessage()
    {
        // Arrange:
        const string ExpectedErrorType = "errorType";
        const string ExpectedErrorMessage = "errorMessage";
        _sortParametersMapperStub
            .Setup(sortParametersMapper => sortParametersMapper.MapCustomerSortParameters(It.IsAny<string>()))
            .Returns(Result.Fail<string>(ExpectedErrorType, ExpectedErrorMessage));

        // Act:
        Result<PaginatedList<GetCustomerDto>> handlerResult = await _getCustomersByQueryParametersQueryHandler.HandleAsync(
            _getCustomersByQueryParametersQuery,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(ExpectedErrorType, handlerResult.ErrorType);
        Assert.Equal(ExpectedErrorMessage, handlerResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithEmptySearchParameter_ReturnsListWithAllCustomers()
    {
        // Arrange:
        Customer customer = TestFixtures.CreateDummyCustomer();
        var expectedGetCustomerDto = new GetCustomerDto(
            customer.Id,
            customer.FullName.ToString(),
            customer.Email.ToString(),
            customer.PhoneNumber.ToString(),
            [],
            customer.CustomerStatus.ToString(),
            customer.CustomerPoints);
        var paginatedCustomers = new PaginatedList<Customer>(
            [customer],
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>());
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Customer>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedCustomers);

        // Act:
        Result<PaginatedList<GetCustomerDto>> handlerResult = await _getCustomersByQueryParametersQueryHandler.HandleAsync(
            _getCustomersByQueryParametersQuery,
            It.IsAny<CancellationToken>());

        // Assert:
        GetCustomerDto actualGetCustomerDto = handlerResult.Value!.FirstOrDefault()!;
        Assert.Equal(expectedGetCustomerDto.Id, actualGetCustomerDto.Id);
        Assert.Equal(expectedGetCustomerDto.FullName, actualGetCustomerDto.FullName);
        Assert.Equal(expectedGetCustomerDto.Email, actualGetCustomerDto.Email);
        Assert.Equal(expectedGetCustomerDto.PhoneNumber, actualGetCustomerDto.PhoneNumber);
        Assert.Equal(expectedGetCustomerDto.Books, actualGetCustomerDto.Books);
        Assert.Equal(expectedGetCustomerDto.CustomerStatus, actualGetCustomerDto.CustomerStatus);
        Assert.Equal(expectedGetCustomerDto.CustomerPoints, actualGetCustomerDto.CustomerPoints);
    }
}
