internal sealed class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, GetCustomerDto>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _getCustomerDtoMapper;

    public GetCustomerByIdQueryHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, GetCustomerDto> getCustomerDtoMapper
    )
    {
        _customerRepository = customerRepository;
        _getCustomerDtoMapper = getCustomerDtoMapper;
    }

    public Task<Result<GetCustomerDto>> HandleAsync(
        GetCustomerByIdQuery getCustomerByIdQuery,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}
