namespace BookRentalManager.Application.Books.Queries;

public sealed record GetBookByIdQuery(Guid Id) : IRequest<GetBookDto>;
