namespace BookRentalManager.UnitTests.Application.Customers.QueryHandlers;

public sealed class GetCustomersByQueryParametersQueryHandlerTests
{
    private readonly Customer _customer;
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly Mock<IMapper<Customer, GetCustomerDto>> _customerToGetCustomerDtoMapperStub;
    private readonly GetCustomersByQueryParametersQueryHandler _getCustomersByQueryParametersQueryHandler;
    private readonly GetCustomerDto _getCustomerDto;
    private readonly PaginatedList<Customer> _paginatedCustomers;

    public GetCustomersByQueryParametersQueryHandlerTests()
    {
        _customer = TestFixtures.CreateDummyCustomer();
        _paginatedCustomers = new PaginatedList<Customer>(new List<Customer> { _customer }, 1, 1, 1, 1);
        _getCustomerDto = new(
            Guid.NewGuid(),
            _customer.FullName,
            _customer.Email,
            _customer.PhoneNumber,
            new List<GetBookRentedByCustomerDto>(),
            _customer.CustomerStatus,
            _customer.CustomerPoints);
        _customerToGetCustomerDtoMapperStub = new();
        _customerRepositoryStub = new();
        _getCustomersByQueryParametersQueryHandler = new(
            _customerRepositoryStub.Object,
            _customerToGetCustomerDtoMapperStub.Object);
        _customerToGetCustomerDtoMapperStub
            .Setup(customerToGetCustomerDtoMapper => customerToGetCustomerDtoMapper.Map(It.IsAny<Customer>()))
            .Returns(_getCustomerDto);
    }

    [Fact]
    public async Task HandleAsync_WithAtLeastOneCustomerWithSearchParameter_ReturnsListWithMatchingCustomer()
    {
        // Arrange:
        var getCustomersByQueryParametersQuery = new GetCustomersByQueryParametersQuery(
            TestFixtures.PageIndex,
            TestFixtures.PageSize,
            _customer.Email.EmailAddress,
            "");
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Customer>>(),
                default))
            .ReturnsAsync(_paginatedCustomers);

        // Act:
        Result<PaginatedList<GetCustomerDto>> handlerResult = await _getCustomersByQueryParametersQueryHandler.HandleAsync(
            getCustomersByQueryParametersQuery,
            default);

        // Assert:
        Assert.Equal(_getCustomerDto, handlerResult.Value.FirstOrDefault());
    }

    [Fact]
    public async Task HandleAsync_WithoutAnyCustomerWithSearchParameter_ReturnsEmptyList()
    {
        // Arrange:
        var getCustomersByQueryParametersQuery = new GetCustomersByQueryParametersQuery(
            TestFixtures.PageIndex,
            TestFixtures.PageSize,
            "test@email.com",
            "");
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Customer>>(),
                default))
            .ReturnsAsync(new PaginatedList<Customer>(new List<Customer>(), 1, 1, 1, 1));

        // Act:
        Result<PaginatedList<GetCustomerDto>> handlerResult = await _getCustomersByQueryParametersQueryHandler.HandleAsync(
            getCustomersByQueryParametersQuery,
            default);

        // Assert:
        Assert.Empty(handlerResult.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task HandleAsync_WithEmptySearchParameter_ReturnsListWithAllCustomers(string searchParam)
    {
        // Arrange:
        var getCustomersByQueryParametersQuery = new GetCustomersByQueryParametersQuery(
            TestFixtures.PageIndex,
            TestFixtures.PageSize,
            searchParam,
            "");
        _paginatedCustomers.Add(new Customer(
            FullName.Create("Sarah", "Smith").Value,
            Email.Create("sarah.smith@email.com").Value,
            PhoneNumber.Create(200, 3_454_763).Value));
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Customer>>(),
                default))
            .ReturnsAsync(_paginatedCustomers);

        // Act:
        Result<PaginatedList<GetCustomerDto>> handlerResult = await _getCustomersByQueryParametersQueryHandler.HandleAsync(
            getCustomersByQueryParametersQuery,
            default);

        // Assert:
        Assert.Equal(2, handlerResult.Value.Count);
    }
}
