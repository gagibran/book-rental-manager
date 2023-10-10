namespace BookRentalManager.UnitTests.Application.Customers.QueryHandlers;

public sealed class GetCustomersByQueryParametersQueryHandlerTests
{
    private readonly GetCustomersByQueryParametersQuery _getCustomersByQueryParametersQuery;
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly Mock<IMapper<Customer, GetCustomerDto>> _customerToGetCustomerDtoMapperStub;
    private readonly Mock<IMapper<CustomerSortParameters, Result<string>>> _customerSortParametersMapperStub;
    private readonly GetCustomersByQueryParametersQueryHandler _getCustomersByQueryParametersQueryHandler;

    public GetCustomersByQueryParametersQueryHandlerTests()
    {
        _getCustomersByQueryParametersQuery = new(
            It.IsAny<int>(),
            It.IsAny<int>(),
            string.Empty,
            It.IsAny<string>());
        _customerToGetCustomerDtoMapperStub = new();
        _customerSortParametersMapperStub = new();
        _customerRepositoryStub = new();
        _getCustomersByQueryParametersQueryHandler = new(
            _customerRepositoryStub.Object,
            _customerToGetCustomerDtoMapperStub.Object,
            _customerSortParametersMapperStub.Object);
        _customerSortParametersMapperStub
            .Setup(customerSortParametersMapper => customerSortParametersMapper.Map(It.IsAny<CustomerSortParameters>()))
            .Returns(Result.Success<string>(string.Empty));
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
                new List<Customer>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()));
        _customerToGetCustomerDtoMapperStub
            .Setup(customerToGetCustomerDtoMapper => customerToGetCustomerDtoMapper.Map(It.IsAny<Customer>()))
            .Returns(new GetCustomerDto(
                It.IsAny<Guid>(),
                FullName.Create("John", "Doe").Value!,
                Email.Create("johndoe@email.com").Value!,
                PhoneNumber.Create(200, 2_000_000).Value!,
                It.IsAny<IReadOnlyList<GetBookRentedByCustomerDto>>(),
                CustomerStatus.Create(11),
                It.IsAny<int>()));

        // Act:
        Result<PaginatedList<GetCustomerDto>> handlerResult = await _getCustomersByQueryParametersQueryHandler.HandleAsync(
            _getCustomersByQueryParametersQuery,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Empty(handlerResult.Value!);
    }

    [Fact]
    public async Task HandleAsync_WithEmptySearchParameter_ReturnsListWithAllCustomers()
    {
        // Arrange:
        Customer customer = TestFixtures.CreateDummyCustomer();
        var expectedGetCustomerDto = new GetCustomerDto(
            Guid.NewGuid(),
            customer.FullName,
            customer.Email,
            customer.PhoneNumber,
            new List<GetBookRentedByCustomerDto>(),
            customer.CustomerStatus,
            customer.CustomerPoints);
        var paginatedCustomers = new PaginatedList<Customer>(
            new List<Customer> { customer },
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
        _customerToGetCustomerDtoMapperStub
            .Setup(customerToGetCustomerDtoMapper => customerToGetCustomerDtoMapper.Map(It.IsAny<Customer>()))
            .Returns(expectedGetCustomerDto);

        // Act:
        Result<PaginatedList<GetCustomerDto>> handlerResult = await _getCustomersByQueryParametersQueryHandler.HandleAsync(
            _getCustomersByQueryParametersQuery,
            It.IsAny<CancellationToken>());

        // Assert (maybe refactor this using FluentAssertions):
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
