using BookRentalManager.Application.CustomerCqrs.Queries;

namespace BookRentalManager.Application.CustomerCqrs.QueryHandlers;

internal sealed class GetAllCustomersQueryHandler
    : IQueryHandler<GetAllCustomersQuery, IReadOnlyList<GetCustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _getCustomerDtoMapper;

    public GetAllCustomersQueryHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, GetCustomerDto> getCustomerDtoMapper
    )
    {
        _customerRepository = customerRepository;
        _getCustomerDtoMapper = getCustomerDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetCustomerDto>>> HandleAsync(
        GetAllCustomersQuery query,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<Customer> customers = await _customerRepository
            .GetAllAsync(cancellationToken);
        if (!customers.Any())
        {
            return Result
                .Fail<IReadOnlyList<GetCustomerDto>>("There are currently no customers registered.");
        }
        IEnumerable<GetCustomerDto> customersDto = from customer in customers
                                                   select _getCustomerDtoMapper.Map(customer);
        return Result.Success<IReadOnlyList<GetCustomerDto>>(customersDto.ToList());
    }
}
