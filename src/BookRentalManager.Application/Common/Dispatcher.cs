using BookRentalManager.Application.Exceptions;

namespace BookRentalManager.Application.Common;

public sealed class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public Dispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<Result> DispatchAsync(ICommand command, CancellationToken cancellationToken)
    {
        Type commandHandlerType = typeof(ICommandHandler<>);
        Type commandType = command.GetType();
        Type commandHandlerGenericType = commandHandlerType.MakeGenericType(commandType);
        var commandHandler = (ICommandHandler<ICommand>?)_serviceProvider
            .GetService(commandHandlerGenericType);
        if (commandHandler is null)
        {
            throw new CommandHandlerObjectCannotBeNullException();
        }
        return await commandHandler.HandleAsync(command, cancellationToken);
    }

    public async Task<Result<TResult>> DispatchAsync<TResult>(
        IQuery<TResult> query,
        CancellationToken cancellationToken
    )
    {
        Type queryHandlerType = typeof(IQueryHandler<,>);
        Type[] queryHandlerArgumentTypes =
        {
            query.GetType(),
            typeof(TResult)
        };
        Type queryHandlerGenericType = queryHandlerType
            .MakeGenericType(queryHandlerArgumentTypes);
        var queryHandler = (IQueryHandler<IQuery<TResult>, TResult>?)_serviceProvider
            .GetService(queryHandlerGenericType);
        if (queryHandler is null)
        {
            throw new QueryHandlerObjectCannotBeNullException();
        }
        return await queryHandler.HandleAsync(query, cancellationToken);
    }
}
