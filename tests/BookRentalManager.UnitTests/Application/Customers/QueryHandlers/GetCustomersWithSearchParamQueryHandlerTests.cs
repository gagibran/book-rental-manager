namespace BookRentalManager.UnitTests.Application.Customers.QueryHandlers;

public sealed class GetCustomersWithSearchParamQueryHandlerTests
{
    private readonly Customer _customer;
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly Mock<IMapper<Customer, GetCustomerDto>> _getCustomerDtoMapperStub;
    private readonly GetCustomersWithSearchParamQueryHandler _getCustomersWithSearchParamQueryHandler;
    private readonly GetCustomerDto _getCustomerDto;
    private readonly List<Customer> _customers;

    public GetCustomersWithSearchParamQueryHandlerTests()
    {
        _customer = TestFixtures.CreateDummyCustomer();
        _customers = new List<Customer> { _customer };
        _getCustomerDto = new(
            Guid.NewGuid(),
            _customer.FullName,
            _customer.Email,
            _customer.PhoneNumber,
            new List<GetCustomerBookDto>(),
            _customer.CustomerStatus,
            _customer.CustomerPoints);
        _getCustomerDtoMapperStub = new();
        _customerRepositoryStub = new();
        _getCustomersWithSearchParamQueryHandler = new(
            _customerRepositoryStub.Object,
            _getCustomerDtoMapperStub.Object);
        _getCustomerDtoMapperStub
            .Setup(getCustomerDtoMapper => getCustomerDtoMapper.Map(It.IsAny<Customer>()))
            .Returns(_getCustomerDto);
    }

    [Fact]
    public async Task HandleAsync_WithAtLeastOneCustomerWithSearchParameter_ReturnsListWithMatchingCustomer()
    {
        // Arrange:
        var getCustomersWithSearchParamQuery = new GetCustomersWithSearchParamQuery(
            TestFixtures.PageIndex,
            TestFixtures.TotalItemsPerPage,
            _customer.Email.EmailAddress);
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Customer>>(),
                default))
            .ReturnsAsync(_customers);

        // Act:
        Result<IReadOnlyList<GetCustomerDto>> handlerResult = await _getCustomersWithSearchParamQueryHandler.HandleAsync(
            getCustomersWithSearchParamQuery,
            default);

        // Assert:
        Assert.Equal(_getCustomerDto, handlerResult.Value.FirstOrDefault());
    }

    [Fact]
    public async Task HandleAsync_WithoutAnyCustomerWithSearchParameter_ReturnsEmptyList()
    {
        // Arrange:
        var getCustomersWithSearchParamQuery = new GetCustomersWithSearchParamQuery(
            TestFixtures.PageIndex,
            TestFixtures.TotalItemsPerPage,
            "test@email.com");
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Customer>>(),
                default))
            .ReturnsAsync(new List<Customer>());

        // Act:
        Result<IReadOnlyList<GetCustomerDto>> handlerResult = await _getCustomersWithSearchParamQueryHandler.HandleAsync(
            getCustomersWithSearchParamQuery,
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
        var getCustomersWithSearchParamQuery = new GetCustomersWithSearchParamQuery(
            TestFixtures.PageIndex,
            TestFixtures.TotalItemsPerPage,
            searchParam);
        _customers.Add(new Customer(
            FullName.Create("Sarah", "Smith").Value,
            Email.Create("sarah.smith@email.com").Value,
            PhoneNumber.Create(200, 3_454_763).Value));
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Customer>>(),
                default))
            .ReturnsAsync(_customers);

        // Act:
        Result<IReadOnlyList<GetCustomerDto>> handlerResult = await _getCustomersWithSearchParamQueryHandler.HandleAsync(
            getCustomersWithSearchParamQuery,
            default);

        // Assert:
        Assert.Equal(2, handlerResult.Value.Count);
    }
}
