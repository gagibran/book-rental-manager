namespace BookRentalManager.UnitTests.Application.Customers.QueryHandlers;

public sealed class GetCustomerByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly Mock<IMapper<Customer, GetCustomerDto>> _customerToGetCustomerDtoMapperStub;
    private readonly GetCustomerByIdQueryHandler _getCustomerByIdQueryHandler;
    private readonly Customer _customer;
    private readonly GetCustomerDto _getCustomerDto;

    public GetCustomerByIdQueryHandlerTests()
    {
        _customer = TestFixtures.CreateDummyCustomer();
        _getCustomerDto = new(
            _customer.Id,
            _customer.FullName,
            _customer.Email,
            _customer.PhoneNumber,
            new List<GetBookRentedByCustomerDto>(),
            _customer.CustomerStatus,
            _customer.CustomerPoints);
        _customerToGetCustomerDtoMapperStub = new();
        _customerRepositoryStub = new();
        _getCustomerByIdQueryHandler = new(
            _customerRepositoryStub.Object,
            _customerToGetCustomerDtoMapperStub.Object);
        _customerToGetCustomerDtoMapperStub
            .Setup(customerToGetCustomerDtoMapper => customerToGetCustomerDtoMapper.Map(It.IsAny<Customer>()))
            .Returns(_getCustomerDto);
    }

    [Fact]
    public async Task HandleAsync_WithCustomerWithId_ReturnsCustomer()
    {
        // Arrange:
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Customer>>(),
                default))
            .ReturnsAsync(_customer);

        // Act:
        Result<GetCustomerDto> handlerResult = await _getCustomerByIdQueryHandler.HandleAsync(
            new GetCustomerByIdQuery(_customer.Id),
            default);

        // Assert:
        Assert.Equal(_getCustomerDto.Id, handlerResult.Value!.Id);
    }

    [Fact]
    public async Task HandleAsync_WithCustomerWithNonexistingId_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = $"No customer with the ID of '{_customer.Id}' was found.";
        _customerRepositoryStub
            .Setup(customerRepository => customerRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Customer>>(),
                default))
            .ReturnsAsync((Customer)null!);

        // Act:
        Result<GetCustomerDto> handlerResult = await _getCustomerByIdQueryHandler.HandleAsync(
            new GetCustomerByIdQuery(_customer.Id),
            default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handlerResult.ErrorMessage);
    }
}
