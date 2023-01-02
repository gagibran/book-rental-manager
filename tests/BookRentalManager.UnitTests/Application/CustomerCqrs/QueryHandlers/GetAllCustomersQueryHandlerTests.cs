using BookRentalManager.Application.CustomerCqrs.Queries;
using BookRentalManager.Application.CustomerCqrs.QueryHandlers;


namespace BookRentalManager.UnitTests.Application.CustomerCqrs.QueryHandlers;

public sealed class GetAllCustomersQueryHandlerTests
{
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly Mock<IMapper<Customer, GetCustomerDto>> _getCustomerDtoMapperStub;
    private readonly GetAllCustomersQueryHandler _getAllCustomersQueryHandler;
    private readonly GetCustomerDto _getCustomerDto;

    public GetAllCustomersQueryHandlerTests()
    {
        Customer customer = TestFixtures.CreateDummyCustomer();
        _getCustomerDto = new(
            Guid.NewGuid(),
            customer.FullName,
            customer.Email,
            customer.PhoneNumber,
            new List<GetCustomerBooksDto>(),
            customer.CustomerStatus,
            customer.CustomerPoints
        );
        _getCustomerDtoMapperStub = new();
        _customerRepositoryStub = new();
        _getAllCustomersQueryHandler = new(
            _customerRepositoryStub.Object,
            _getCustomerDtoMapperStub.Object
        );
        _getCustomerDtoMapperStub
            .Setup(getCustomerDtoMapper => getCustomerDtoMapper.Map(It.IsAny<Customer>()))
            .Returns(_getCustomerDto);
    }

    [Fact]
    public async Task HandleAsync_WithoutAnyCustomers_ReturnsErrorMessage()
    {
        // Assert:
        var expectedErrorMessage = "There are currently no customers registered.";
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetAllAsync(default(CancellationToken)))
            .ReturnsAsync(new List<Customer>());

        // Act:
        Result<IReadOnlyList<GetCustomerDto>> handlerResult = await _getAllCustomersQueryHandler
            .HandleAsync(new GetAllCustomersQuery(), default(CancellationToken));

        // Assert:
        Assert.Equal(expectedErrorMessage, handlerResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithAtLeastOneCustomers_ReturnsListWithCustomer()
    {
        // Assert:
        var expectedListOfCustomers = new List<Customer>
        {
            TestFixtures.CreateDummyCustomer()
        };
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetAllAsync(default(CancellationToken)))
            .ReturnsAsync(expectedListOfCustomers);

        // Act:
        Result<IReadOnlyList<GetCustomerDto>> handlerResult = await _getAllCustomersQueryHandler
            .HandleAsync(new GetAllCustomersQuery(), default(CancellationToken));

        // Assert:
        Assert.Equal(_getCustomerDto, handlerResult.Value.FirstOrDefault());
    }
}
