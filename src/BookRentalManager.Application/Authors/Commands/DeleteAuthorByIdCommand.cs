namespace BookRentalManager.Application.Authors.Commands;

public sealed record DeleteAuthorByIdCommand(Guid Id) : IRequest;
