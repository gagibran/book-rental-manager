namespace BookRentalManager.Application.Authors.Queries;

public sealed class GetAuthorByIdQuery(Guid id) : IRequest<GetAuthorDto>
{
    public Guid Id { get; } = id;
}
