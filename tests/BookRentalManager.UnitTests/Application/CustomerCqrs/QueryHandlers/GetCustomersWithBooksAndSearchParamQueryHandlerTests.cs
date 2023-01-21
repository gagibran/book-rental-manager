namespace BookRentalManager.UnitTests.Application.CustomerCqrs.QueryHandlers;

public sealed class GetCustomersWithBooksAndSearchParamQueryHandlerTests
{
    private readonly Customer _customer;
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly Mock<IMapper<Customer, GetCustomerDto>> _getCustomerDtoMapperStub;
    private readonly GetCustomersWithBooksAndSearchParamQueryHandler _getCustomersWithBooksAndSearchParamQueryHandler;
    private readonly GetCustomerDto _getCustomerDto;
    private readonly List<Customer> _customers;

    public GetCustomersWithBooksAndSearchParamQueryHandlerTests()
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
        _getCustomersWithBooksAndSearchParamQueryHandler = new(
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
        var getCustomersWithBooksAndSearchParamQuery = new GetCustomersWithBooksAndSearchParamQuery(
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
        Result<IReadOnlyList<GetCustomerDto>> handlerResult = await _getCustomersWithBooksAndSearchParamQueryHandler.HandleAsync(
            getCustomersWithBooksAndSearchParamQuery,
            default);

        // Assert:
        Assert.Equal(_getCustomerDto, handlerResult.Value.FirstOrDefault());
    }

    [Fact]
    public async Task HandleAsync_WithoutAnyCustomerWithSearchParameter_ReturnsEmptyList()
    {
        // Arrange:
        var getCustomersWithBooksAndSearchParamQuery = new GetCustomersWithBooksAndSearchParamQuery(
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
        Result<IReadOnlyList<GetCustomerDto>> handlerResult = await _getCustomersWithBooksAndSearchParamQueryHandler.HandleAsync(
            getCustomersWithBooksAndSearchParamQuery,
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
        var getCustomersWithBooksAndSearchParamQuery = new GetCustomersWithBooksAndSearchParamQuery(
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
        Result<IReadOnlyList<GetCustomerDto>> handlerResult = await _getCustomersWithBooksAndSearchParamQueryHandler.HandleAsync(
            getCustomersWithBooksAndSearchParamQuery,
            default);

        // Assert:
        Assert.Equal(2, handlerResult.Value.Count);
    }
}
