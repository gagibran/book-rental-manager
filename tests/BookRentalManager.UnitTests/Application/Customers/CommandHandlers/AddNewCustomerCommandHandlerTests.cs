using BookRentalManager.Application.Customers.Commands;
using BookRentalManager.Application.Customers.CommandHandlers;

namespace BookRentalManager.UnitTests.Application.Customers.CommandHandlers;

public sealed class AddNewCustomerCommandHandlerTests
{
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly FullName _fullName;
    private readonly Email _email;
    private readonly PhoneNumber _phoneNumber;
    private readonly AddNewCustomerCommand _addNewCustomerCommand;
    private readonly AddNewCustomerCommandHandler _addNewCustomerCommandHandler;

    public AddNewCustomerCommandHandlerTests()
    {
        _customerRepositoryStub = new();
        _fullName = FullName.Create("John", "Doe").Value;
        _email = Email.Create("john.doe@email.com").Value;
        _phoneNumber = PhoneNumber.Create(200, 2_000_000).Value;
        _addNewCustomerCommand = new(new Customer(_fullName, _email, _phoneNumber));
        _addNewCustomerCommandHandler = new(_customerRepositoryStub.Object);
    }

    [Fact]
    public async Task HandleAsync_WithExistingCustomerEmail_ReturnsErrorMessage()
    {
        // Arrange:
        var customer = new Customer(_fullName, _email, _phoneNumber);
        var expectedErrorMessage = "A customer with the email 'john.doe@email.com' already exists.";
        _customerRepositoryStub
            .Setup(customerRepository =>
                customerRepository.GetFirstOrDefaultBySpecificationAsync(
                    It.IsAny<Specification<Customer>>(),
                    default))
            .ReturnsAsync(customer);

        // Act:
        Result handleResult = await _addNewCustomerCommandHandler.HandleAsync(_addNewCustomerCommand, default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handleResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingCustomerEmail_ReturnsSuccess()
    {
        // Arrange:
        _customerRepositoryStub
            .Setup(customerRepository =>
                customerRepository.CreateAsync(
                    It.IsAny<Customer>(),
                    default))
            .Verifiable();

        // Act:
        Result handleResult = await _addNewCustomerCommandHandler.HandleAsync(_addNewCustomerCommand, default);

        // Assert:
        Assert.True(handleResult.IsSuccess);
    }
}