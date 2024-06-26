using BookRentalManager.Application.Exceptions;

namespace BookRentalManager.UnitTests.Application.Common;

public sealed class DispatcherTests
{
    private readonly Mock<IServiceProvider> _serviceProviderStub;
    private readonly Mock<IRequest> _requestStub;
    private readonly Mock<IRequest<Customer>> _requestResponseStub;
    private readonly Dispatcher _dispatcher;
    private readonly Customer _customer;

    public DispatcherTests()
    {
        _serviceProviderStub = new();
        _requestStub = new();
        _requestResponseStub = new();
        _dispatcher = new(_serviceProviderStub.Object);
        _customer = TestFixtures.CreateDummyCustomer();
    }

    [Fact]
    public async Task DispatchAsync_WithValidCommandHandler_ReturnsSuccess()
    {
        // Arrange:
        var requestHandlerStub = new Mock<IRequestHandler<IRequest>>();
        requestHandlerStub
            .Setup(requestHandler => requestHandler.HandleAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        _serviceProviderStub
            .Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
            .Returns(requestHandlerStub.Object);

        // Act:
        Result dispatcherResult = await _dispatcher.DispatchAsync(_requestStub.Object, It.IsAny<CancellationToken>());

        // Assert:
        Assert.True(dispatcherResult.IsSuccess);
    }

    [Fact]
    public async Task DispatchAsync_WithNullCommandHandler_ThrowsException()
    {
        // Arrange:
        _serviceProviderStub
            .Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
            .Returns(null!);

        // Assert:
        await Assert.ThrowsAsync<HandlerObjectCannotBeNullException>(() => _dispatcher.DispatchAsync(
            new DeleteCustomerByIdCommand(It.IsAny<Guid>()),
            It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task DispatchAsync_WithValidCommandHandlerReturningResult_ReturnsSuccess()
    {
        // Arrange:
        var requestHandlerStub = new Mock<IRequestHandler<IRequest<Customer>, Customer>>();
        var requestStub = new Mock<IRequest<Customer>>();
        requestHandlerStub
            .Setup(requestHandler => requestHandler.HandleAsync(It.IsAny<IRequest<Customer>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(_customer));
        _serviceProviderStub
            .Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
            .Returns(requestHandlerStub.Object);

        // Act:
        Result<Customer> dispatcherResult = await _dispatcher.DispatchAsync(requestStub.Object, default);

        // Assert:
        Assert.True(dispatcherResult.IsSuccess);
        Assert.Equal(_customer, dispatcherResult.Value);
    }

    [Fact]
    public async Task DispatchAsync_WithNullCommandHandlerReturningResult_ThrowsException()
    {
        // Arrange:
        _serviceProviderStub
            .Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
            .Returns(null!);

        // Assert:
        await Assert.ThrowsAsync<HandlerObjectCannotBeNullException>(() => _dispatcher.DispatchAsync(
            new CreateCustomerCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<PhoneNumberDto>()),
            It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task DispatchAsync_WithValidQueryHandler_ReturnsSuccess()
    {
        // Arrange:
        var requestResponseHandlerStub = new Mock<IRequestHandler<IRequest<Customer>, Customer>>();
        requestResponseHandlerStub
            .Setup(requestResponseHandler =>
                requestResponseHandler.HandleAsync(
                    It.IsAny<IRequest<Customer>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(_customer));
        _serviceProviderStub
            .Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
            .Returns(requestResponseHandlerStub.Object);

        // Act:
        Result dispatcherResult = await _dispatcher.DispatchAsync(_requestResponseStub.Object, default);

        // Assert:
        Assert.True(dispatcherResult.IsSuccess);
    }

    [Fact]
    public async Task DispatchAsync_WithNullQueryHandler_ThrowsException()
    {
        // Arrange:
        _serviceProviderStub
            .Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
            .Returns(null!);

        // Assert:
        await Assert.ThrowsAsync<HandlerObjectCannotBeNullException>(() => _dispatcher.DispatchAsync(
            new GetCustomerByIdQuery(It.IsAny<Guid>()),
            It.IsAny<CancellationToken>()));
    }
}
