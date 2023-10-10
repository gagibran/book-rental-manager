namespace BookRentalManager.UnitTests.Application.Decorators;

public sealed class HandlerLoggingDecoratorTests
{
    private readonly Mock<IRequestHandler<IRequest>> _requestHandlerStub;
    private readonly Mock<ILogger<IRequestHandler<IRequest>>> _loggerStub;
    private readonly Mock<IRequestHandler<IRequest<int>, int>> _requestHandlerWithResultStub;
    private readonly Mock<ILogger<IRequestHandler<IRequest<int>, int>>> _loggerWithResultStub;
    private readonly HandlerLoggingDecorator<IRequest> _handlerLoggingDecorator;
    private readonly HandlerLoggingDecorator<IRequest<int>, int> _handlerLoggingWithResultDecorator;

    public HandlerLoggingDecoratorTests()
    {
        _requestHandlerStub = new();
        _loggerStub = new();
        _requestHandlerWithResultStub = new();
        _loggerWithResultStub = new();
        _handlerLoggingDecorator = new(_requestHandlerStub.Object, _loggerStub.Object);
        _handlerLoggingWithResultDecorator = new(_requestHandlerWithResultStub.Object, _loggerWithResultStub.Object);
        _loggerStub.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _loggerWithResultStub.Setup(loggerWithResult => loggerWithResult.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
    }

    [Fact]
    public async Task HandleAsync_WithDecoratedHandleAsyncReturningSuccess_CallsLogInformationTwice()
    {
        // Arrange:
        _requestHandlerStub
            .Setup(requestHandler => requestHandler.HandleAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act:
        await _handlerLoggingDecorator.HandleAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>());

        // Assert:
        _loggerStub.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task HandleAsync_WithDecoratedHandleAsyncReturningFail_CallsLogErrorOnce()
    {
        // Arrange:
        _requestHandlerStub
            .Setup(requestHandler => requestHandler.HandleAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("errorType", "errorMessage"));

        // Act:
        await _handlerLoggingDecorator.HandleAsync(It.IsAny<IRequest>(), It.IsAny<CancellationToken>());

        // Assert:
        _loggerStub.Verify(
            logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once());
    }

    [Fact]
    public async Task HandleAsync_WithDecoratedHandleWithReturningResultAsyncReturningSuccess_CallsLogInformationThrice()
    {
        // Arrange:
        _requestHandlerWithResultStub
            .Setup(requestHandlerWithResult => requestHandlerWithResult.HandleAsync(
                It.IsAny<IRequest<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<int>(0));

        // Act:
        await _handlerLoggingWithResultDecorator.HandleAsync(It.IsAny<IRequest<int>>(), It.IsAny<CancellationToken>());

        // Assert:
        _loggerWithResultStub.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Exactly(3));
    }

    [Fact]
    public async Task HandleAsync_WithDecoratedHandleWithReturningResultAsyncReturningFail_CallsLogErrorOnce()
    {
        // Arrange:
        _requestHandlerWithResultStub
            .Setup(requestHandlerWithResult => requestHandlerWithResult.HandleAsync(
                It.IsAny<IRequest<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<int>("errorType", "errorMessage"));

        // Act:
        await _handlerLoggingWithResultDecorator.HandleAsync(It.IsAny<IRequest<int>>(), It.IsAny<CancellationToken>());

        // Assert:
        _loggerWithResultStub.Verify(
            logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once());
    }
}
