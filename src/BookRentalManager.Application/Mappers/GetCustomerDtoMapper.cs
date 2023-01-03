namespace BookRentalManager.Application.Mappers;

internal sealed class GetCustomerDtoMapper : IMapper<Customer, GetCustomerDto>
{
    private readonly IMapper<IReadOnlyList<Book>, IReadOnlyList<GetCustomerBooksDto>> _getCustomerBooksDtoMapper;

    public GetCustomerDtoMapper(
        IMapper<IReadOnlyList<Book>, IReadOnlyList<GetCustomerBooksDto>> getCustomerBooksDto
    )
    {
        _getCustomerBooksDtoMapper = getCustomerBooksDto;
    }

    public GetCustomerDto Map(Customer customer)
    {
        return new GetCustomerDto(
            customer.Id,
            customer.FullName,
            customer.Email,
            customer.PhoneNumber,
            _getCustomerBooksDtoMapper.Map(customer.Books),
            customer.CustomerStatus,
            customer.CustomerPoints
        );
    }
}
