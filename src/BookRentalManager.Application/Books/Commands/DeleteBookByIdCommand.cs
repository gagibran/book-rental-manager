namespace BookRentalManager.Application.Books.Commands;

public sealed record DeleteBookByIdCommand(Guid Id) : IRequest;
