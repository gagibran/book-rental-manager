using BookRentalManager.Application.Common;
using BookRentalManager.Application.Exceptions;

namespace BookRentalManager.UnitTests.Application.Common;

public sealed class DispatcherTests
{
    private readonly Mock<IServiceProvider> _serviceProviderStub;
    private readonly Mock<ICommand> _commandStub;
    private readonly Mock<IQuery<Customer>> _queryStub;
    private readonly Dispatcher _dispatcher;
    private readonly Customer _customer;

    public DispatcherTests()
    {
        _serviceProviderStub = new();
        _commandStub = new();
        _queryStub = new();
        _dispatcher = new(_serviceProviderStub.Object);
        _customer = TestFixtures.CreateDummyCustomer();
    }

    [Fact]
    public async Task DispatchAsync_WithValidCommandHandler_ReturnsSuccess()
    {
        // Arrange:
        var commandHandlerStub = new Mock<ICommandHandler<ICommand>>();
        commandHandlerStub
            .Setup(commandHandler => commandHandler.HandleAsync(It.IsAny<ICommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        _serviceProviderStub
            .Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
            .Returns(commandHandlerStub.Object);

        // Act:
        Result dispatcherResult = await _dispatcher.DispatchAsync(_commandStub.Object, default);

        // Assert:
        Assert.True(dispatcherResult.IsSuccess);
    }

    [Fact]
    public void DispatchAsync_WithNullCommandHandler_ThrowsException()
    {
        // Arrange:
        _serviceProviderStub
            .Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
            .Returns(null);

        // Assert:
        Assert.ThrowsAsync<CommandHandlerObjectCannotBeNullException>(
            () => _dispatcher.DispatchAsync(It.IsAny<ICommand>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task DispatchAsync_WithValidCommandHandlerReturningResult_ReturnsSuccess()
    {
        // Arrange:
        var commandHandlerStub = new Mock<ICommandHandler<ICommand<Customer>, Customer>>();
        var commandStub = new Mock<ICommand<Customer>>();
        commandHandlerStub
            .Setup(commandHandler => commandHandler.HandleAsync(It.IsAny<ICommand<Customer>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Customer>(_customer));
        _serviceProviderStub
            .Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
            .Returns(commandHandlerStub.Object);

        // Act:
        Result<Customer> dispatcherResult = await _dispatcher.DispatchAsync<Customer>(commandStub.Object, default);

        // Assert:
        Assert.True(dispatcherResult.IsSuccess);
        Assert.Equal(_customer, dispatcherResult.Value);
    }

    [Fact]
    public void DispatchAsync_WithNullCommandHandlerReturningResult_ThrowsException()
    {
        // Arrange:
        _serviceProviderStub
            .Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
            .Returns(null);

        // Assert:
        Assert.ThrowsAsync<CommandHandlerObjectCannotBeNullException>(
            () => _dispatcher.DispatchAsync<Customer>(It.IsAny<ICommand<Customer>>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task DispatchAsync_WithValidQueryHandler_ReturnsSuccess()
    {
        // Arrange:
        var queryHandlerStub = new Mock<IQueryHandler<IQuery<Customer>, Customer>>();
        queryHandlerStub
            .Setup(queryHandler =>
                queryHandler.HandleAsync(
                    It.IsAny<IQuery<Customer>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Customer>(_customer));
        _serviceProviderStub
            .Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
            .Returns(queryHandlerStub.Object);

        // Act:
        Result dispatcherResult = await _dispatcher.DispatchAsync<Customer>(_queryStub.Object, default);

        // Assert:
        Assert.True(dispatcherResult.IsSuccess);
    }

    [Fact]
    public void DispatchAsync_WithNullQueryHandler_ThrowsException()
    {
        // Arrange:
        _serviceProviderStub
            .Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
            .Returns(null);

        // Assert:
        Assert.ThrowsAsync<QueryHandlerObjectCannotBeNullException>(
            () => _dispatcher.DispatchAsync(
                It.IsAny<IQuery<Customer>>(),
                It.IsAny<CancellationToken>()));
    }
}
