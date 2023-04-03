namespace BookRentalManager.UnitTests.Application.Decorators;

public sealed class ExecutionTimeLoggingDecoratorTests
{
    private readonly Mock<IRequestHandler<IRequest>> _requestHandlerStub;
    private readonly Mock<ILogger<IRequestHandler<IRequest>>> _loggerStub;
    private readonly Mock<IRequestHandler<IRequest<int>, int>> _requestHandlerWithResultStub;
    private readonly Mock<ILogger<IRequestHandler<IRequest<int>, int>>> _loggerWithResultStub;
    private readonly ExecutionTimeLoggingDecorator<IRequest> _executionTimeLoggingDecorator;
    private readonly ExecutionTimeLoggingDecorator<IRequest<int>, int> _executionTimeLoggingWithResultDecorator;

    public ExecutionTimeLoggingDecoratorTests()
    {
        _requestHandlerStub = new();
        _loggerStub = new();
        _requestHandlerWithResultStub = new();
        _loggerWithResultStub = new();
        _executionTimeLoggingDecorator = new(_requestHandlerStub.Object, _loggerStub.Object);
        _executionTimeLoggingWithResultDecorator = new(_requestHandlerWithResultStub.Object, _loggerWithResultStub.Object);
    }

    [Fact]
    public async Task HandleAsync_WithExpectedParameters_CallsLogDebugOnce()
    {
        // Arrange:
        _requestHandlerStub
            .Setup(requestHandler => requestHandler.HandleAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act:
        await _executionTimeLoggingDecorator.HandleAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>());

        // Assert:
        _loggerStub.Verify(
            logger => logger.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithExpectedParametersForReturningResult_CallsLogDebugOnce()
    {
        // Arrange:
        _requestHandlerWithResultStub
            .Setup(requestHandlerWithResult => requestHandlerWithResult.HandleAsync(
                It.IsAny<IRequest<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<int>(0));

        // Act:
        await _executionTimeLoggingWithResultDecorator.HandleAsync(It.IsAny<IRequest<int>>(), It.IsAny<CancellationToken>());

        // Assert:
        _loggerWithResultStub.Verify(
            logger => logger.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }
}
