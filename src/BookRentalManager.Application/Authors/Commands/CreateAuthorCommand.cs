namespace BookRentalManager.Application.Authors.Commands;

public sealed record CreateAuthorCommand(string FirstName, string LastName) : IRequest<AuthorCreatedDto>;
