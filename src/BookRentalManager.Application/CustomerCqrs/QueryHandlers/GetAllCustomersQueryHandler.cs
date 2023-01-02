using BookRentalManager.Application.CustomerCqrs.Queries;

namespace BookRentalManager.Application.CustomerCqrs.QueryHandlers;

internal sealed class GetAllCustomersQueryHandler
    : IQueryHandler<GetAllCustomersQuery, IReadOnlyList<Customer>>
{
    private readonly IRepository<Customer> _customerRepository;

    public GetAllCustomersQueryHandler(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<IReadOnlyList<Customer>>> HandleAsync(
        GetAllCustomersQuery query,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<Customer> customers = await _customerRepository
            .GetAllAsync(cancellationToken);
        if (!customers.Any())
        {
            return Result
                .Fail<IReadOnlyList<Customer>>("There are currently no customers registered.");
        }
        return Result.Success<IReadOnlyList<Customer>>(customers);
    }
}
