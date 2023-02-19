using BookRentalManager.Application.Customers.Commands;
using BookRentalManager.Application.Customers.CommandHandlers;

namespace BookRentalManager.UnitTests.Application.Customers.CommandHandlers;

public sealed class CreateCustomerCommandHandlerTests
{
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly Mock<IMapper<Customer, CustomerCreatedDto>> _customerToCustomerCreatedDtoMapperStub;
    private readonly CustomerCreatedDto _customerCreatedDto;
    private readonly CreateCustomerCommand _createCustomerCommand;
    private readonly CreateCustomerCommandHandler _createCustomerCommandHandler;

    public CreateCustomerCommandHandlerTests()
    {
        Customer customer = TestFixtures.CreateDummyCustomer();
        _customerRepositoryStub = new();
        _customerToCustomerCreatedDtoMapperStub = new();
        _createCustomerCommand = new("John", "Doe", "john.doe@email.com", new PhoneNumberDto(200, 2_000_000));
        _customerCreatedDto = new(
            customer.Id,
            customer.FullName.CompleteName,
            customer.Email.EmailAddress,
            customer.PhoneNumber.CompletePhoneNumber,
            customer.CustomerStatus.CustomerType.ToString(),
            customer.CustomerPoints);
        _createCustomerCommandHandler = new(_customerRepositoryStub.Object, _customerToCustomerCreatedDtoMapperStub.Object);
        _customerToCustomerCreatedDtoMapperStub
            .Setup(customerToCustomerCreatedDtoMapper => customerToCustomerCreatedDtoMapper.Map(It.IsAny<Customer>()))
            .Returns(_customerCreatedDto);
        _customerRepositoryStub
            .Setup(customerRepository =>
                customerRepository.GetFirstOrDefaultBySpecificationAsync(
                    It.IsAny<Specification<Customer>>(),
                    default))
            .ReturnsAsync(customer);
    }

    [Fact]
    public async Task HandleAsync_WithExistingCustomerEmail_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = "A customer with the email 'john.doe@email.com' already exists.";

        // Act:
        Result handleResult = await _createCustomerCommandHandler.HandleAsync(_createCustomerCommand, default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handleResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingCustomerEmail_ReturnsSuccess()
    {
        // Arrange:
        _customerRepositoryStub
            .Setup(customerRepository =>
                customerRepository.GetFirstOrDefaultBySpecificationAsync(
                    It.IsAny<Specification<Customer>>(),
                    default))
            .ReturnsAsync((Customer)null!);
        _customerRepositoryStub
            .Setup(customerRepository =>
                customerRepository.CreateAsync(
                    It.IsAny<Customer>(),
                    default))
            .Verifiable();

        // Act:
        Result handleResult = await _createCustomerCommandHandler.HandleAsync(_createCustomerCommand, default);

        // Assert:
        Assert.True(handleResult.IsSuccess);
    }
}
