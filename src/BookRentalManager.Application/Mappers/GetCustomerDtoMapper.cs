namespace BookRentalManager.Application.Mappers;

internal sealed class GetCustomerDtoMapper : IMapper<Customer, GetCustomerDto>
{
    private readonly IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookRentedByCustomerDto>> _getBookRentedByCustomerDtosMapper;

    public GetCustomerDtoMapper(IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookRentedByCustomerDto>> getBookRentedByCustomerDtosMapper)
    {
        _getBookRentedByCustomerDtosMapper = getBookRentedByCustomerDtosMapper;
    }

    public GetCustomerDto Map(Customer customer)
    {
        return new GetCustomerDto(
            customer.Id,
            customer.FullName,
            customer.Email,
            customer.PhoneNumber,
            _getBookRentedByCustomerDtosMapper.Map(customer.Books),
            customer.CustomerStatus,
            customer.CustomerPoints);
    }
}
