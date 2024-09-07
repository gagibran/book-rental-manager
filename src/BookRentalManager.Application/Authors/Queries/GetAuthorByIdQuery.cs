namespace BookRentalManager.Application.Authors.Queries;

public sealed record GetAuthorByIdQuery(Guid Id) : IRequest<GetAuthorDto>;
