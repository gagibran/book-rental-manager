namespace BookRentalManager.Application.Interfaces;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand
{
    Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
