namespace BookRentalManager.Application.DtoMappers;

internal sealed class CustomerToGetCustomerDtoMapper : IMapper<Customer, GetCustomerDto>
{
    private readonly IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookRentedByCustomerDto>> _booksToGetBookRentedByCustomerDtosMapper;

    public CustomerToGetCustomerDtoMapper(IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookRentedByCustomerDto>> booksToGetBookRentedByCustomerDtosMapper)
    {
        _booksToGetBookRentedByCustomerDtosMapper = booksToGetBookRentedByCustomerDtosMapper;
    }

    public GetCustomerDto Map(Customer customer)
    {
        return new GetCustomerDto(
            customer.Id,
            customer.FullName,
            customer.Email,
            customer.PhoneNumber,
            _booksToGetBookRentedByCustomerDtosMapper.Map(customer.Books),
            customer.CustomerStatus,
            customer.CustomerPoints);
    }
}
