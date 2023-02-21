namespace BookRentalManager.Application.Authors.Queries;

public sealed class GetAuthorByIdQuery : IQuery<GetAuthorDto>
{
    public Guid Id { get; }

    public GetAuthorByIdQuery(Guid id)
    {
        Id = id;
    }
}
