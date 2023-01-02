using BookRentalManager.Application.CustomerCqrs.Queries;
using BookRentalManager.Application.CustomerCqrs.QueryHandlers;

namespace BookRentalManager.UnitTests.Application.CustomerCqrs.QueryHandlers;

public sealed class GetAllCustomersQueryHandlerTests
{
    private readonly Mock<IRepository<Customer>> _customerRepositoryStub;
    private readonly GetAllCustomersQueryHandler _getAllCustomersQueryHandler;

    public GetAllCustomersQueryHandlerTests()
    {
        _customerRepositoryStub = new();
        _getAllCustomersQueryHandler = new(_customerRepositoryStub.Object);
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
        Result<IReadOnlyList<Customer>> handlerResult = await _getAllCustomersQueryHandler
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
        Result<IReadOnlyList<Customer>> handlerResult = await _getAllCustomersQueryHandler
            .HandleAsync(new GetAllCustomersQuery(), default(CancellationToken));

        // Assert:
        Assert.Equal(
            expectedListOfCustomers.FirstOrDefault(),
            handlerResult.Value.FirstOrDefault()
        );
    }
}
