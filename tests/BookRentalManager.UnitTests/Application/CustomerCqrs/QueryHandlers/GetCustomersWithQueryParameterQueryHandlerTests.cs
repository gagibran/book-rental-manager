namespace BookRentalManager.UnitTests.Application.CustomerCqrs.QueryHandlers;

public sealed class GetCustomersWithQueryParameterQueryHandlerTests
{
    private readonly int _pageIndex;
    private readonly int _totalItemsPerPage;
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly Mock<IMapper<Customer, GetCustomerDto>> _getCustomerDtoMapperStub;
    private readonly GetCustomersWithQueryParameterQueryHandler _getCustomersQueryHandler;
    private readonly GetCustomerDto _getCustomerDto;

    public GetCustomersWithQueryParameterQueryHandlerTests()
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
            customer.CustomerPoints
        );
        _getCustomerDtoMapperStub = new();
        _customerRepositoryStub = new();
        _getCustomersQueryHandler = new(
            _customerRepositoryStub.Object,
            _getCustomerDtoMapperStub.Object
        );
        _getCustomerDtoMapperStub
            .Setup(getCustomerDtoMapper => getCustomerDtoMapper.Map(It.IsAny<Customer>()))
            .Returns(_getCustomerDto);
    }

    [Fact]
    public async Task HandleAsync_WithAtLeastOneCustomerWithQueryParameter_ReturnsListWithCustomer()
    {
        // Assert:
        var expectedCustomer = TestFixtures.CreateDummyCustomer();
        var expectedListOfCustomers = new List<Customer> { expectedCustomer };
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetAllAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Customer>>(),
                default
            ))
            .ReturnsAsync(expectedListOfCustomers);

        // Act:
        Result<IReadOnlyList<GetCustomerDto>> handlerResult = await _getCustomersQueryHandler.HandleAsync(
            new GetCustomersWithQueryParameterQuery(
                _pageIndex,
                _totalItemsPerPage,
                expectedCustomer.Email.EmailAddress
            ),
            default
        );

        // Assert:
        Assert.Equal(_getCustomerDto, handlerResult.Value.FirstOrDefault());
    }

    [Fact]
    public async Task HandleAsync_WithoutAnyCustomerWithQueryParameter_ReturnsErrorMessage()
    {
        // Assert:
        var expectedErrorMessage = "There are currently no customers containing the value 'test@email.com' registered.";
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetAllAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Customer>>(),
                default
            ))
            .ReturnsAsync(new List<Customer>());

        // Act:
        Result<IReadOnlyList<GetCustomerDto>> handlerResult = await _getCustomersQueryHandler.HandleAsync(
            new GetCustomersWithQueryParameterQuery(
                _pageIndex,
                _totalItemsPerPage,
                "test@email.com"
            ),
            default
        );

        // Assert:
        Assert.Equal(expectedErrorMessage, handlerResult.ErrorMessage);
    }
}
