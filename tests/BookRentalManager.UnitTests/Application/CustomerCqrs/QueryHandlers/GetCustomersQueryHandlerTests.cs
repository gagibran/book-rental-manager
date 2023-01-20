namespace BookRentalManager.UnitTests.Application.CustomerCqrs.QueryHandlers;

public sealed class GetCustomersQueryHandlerTests
{
    private readonly int _pageIndex;
    private readonly int _totalItemsPerPage;
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly Mock<IMapper<Customer, GetCustomerDto>> _getCustomerDtoMapperStub;
    private readonly GetCustomersQueryHandler _getCustomersQueryHandler;
    private readonly GetCustomerDto _getCustomerDto;

    public GetCustomersQueryHandlerTests()
    {
        _pageIndex = 1;
        _totalItemsPerPage = 50;
        Customer customer = TestFixtures.CreateDummyCustomer();
        _getCustomerDto = new(
            Guid.NewGuid(),
            customer.FullName,
            customer.Email,
            customer.PhoneNumber,
            new List<GetCustomerBookDto>(),
            customer.CustomerStatus,
            customer.CustomerPoints);
        _getCustomerDtoMapperStub = new();
        _customerRepositoryStub = new();
        _getCustomersQueryHandler = new(
            _customerRepositoryStub.Object,
            _getCustomerDtoMapperStub.Object);
        _getCustomerDtoMapperStub
            .Setup(getCustomerDtoMapper => getCustomerDtoMapper.Map(It.IsAny<Customer>()))
            .Returns(_getCustomerDto);
    }

    [Fact]
    public async Task HandleAsync_WithoutAnyCustomers_ReturnsErrorMessage()
    {
        // Arrange:
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetAllAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Customer>>(),
                default))
            .ReturnsAsync(new List<Customer>());

        // Act:
        Result<IReadOnlyList<GetCustomerDto>> handlerResult = await _getCustomersQueryHandler.HandleAsync(
            new GetCustomersQuery(_pageIndex, _totalItemsPerPage),
            default);

        // Assert:
        Assert.Empty(handlerResult.Value);
    }

    [Fact]
    public async Task HandleAsync_WithAtLeastOneCustomer_ReturnsListWithCustomer()
    {
        // Arrange:
        var expectedListOfCustomers = new List<Customer>
        {
            TestFixtures.CreateDummyCustomer()
        };
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetAllAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Customer>>(),
                default))
            .ReturnsAsync(expectedListOfCustomers);

        // Act:
        Result<IReadOnlyList<GetCustomerDto>> handlerResult = await _getCustomersQueryHandler.HandleAsync(
            new GetCustomersQuery(_pageIndex, _totalItemsPerPage),
            default);

        // Assert:
        Assert.Equal(_getCustomerDto, handlerResult.Value.FirstOrDefault());
    }
}
