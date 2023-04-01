using Microsoft.Extensions.Logging;

namespace BookRentalManager.Application.Decorators;

public sealed class LoggingCommandDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _commandHandler;
    private readonly ILogger<ICommandHandler<TCommand>> _logger;

    public LoggingCommandDecorator(
        ICommandHandler<TCommand> commandHandler,
        ILogger<ICommandHandler<TCommand>> logger)
    {
        _commandHandler = commandHandler;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Test");
        Result handleAsyncResult = await _commandHandler.HandleAsync(command, cancellationToken);
        _logger.LogInformation(handleAsyncResult.IsSuccess.ToString());
        return handleAsyncResult;
    }
}

public sealed class LoggingCommandDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _commandHandler;
    private readonly ILogger<ICommandHandler<TCommand, TResult>> _logger;

    public LoggingCommandDecorator(
        ICommandHandler<TCommand, TResult> commandHandler,
        ILogger<ICommandHandler<TCommand, TResult>> logger)
    {
        _commandHandler = commandHandler;
        _logger = logger;
    }

    public async Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Test");
        Result<TResult> handleAsyncResult = await _commandHandler.HandleAsync(command, cancellationToken);
        _logger.LogInformation(handleAsyncResult.IsSuccess.ToString());
        return handleAsyncResult;
    }
}


public sealed class LoggingQueryDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    private readonly IQueryHandler<TQuery, TResult> _queryHandler;
    private readonly ILogger<IQueryHandler<TQuery, TResult>> _logger;

    public LoggingQueryDecorator(
        IQueryHandler<TQuery, TResult> queryHandler,
        ILogger<IQueryHandler<TQuery, TResult>> logger)
    {
        _queryHandler = queryHandler;
        _logger = logger;
    }

    public async Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Test");
        Result<TResult> handleAsyncResult = await _queryHandler.HandleAsync(query, cancellationToken);
        _logger.LogInformation(handleAsyncResult.IsSuccess.ToString());
        return handleAsyncResult;
    }
}
