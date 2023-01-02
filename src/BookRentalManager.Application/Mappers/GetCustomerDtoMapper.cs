namespace BookRentalManager.Application.Mappers;

internal sealed class GetCustomerDtoMapper : IMapper<Customer, GetCustomerDto>
{
    private readonly IMapper<IReadOnlyList<Book>, IReadOnlyList<GetCustomerBooksDto>> _getCustomerBooksDto;

    public GetCustomerDtoMapper(
        IMapper<IReadOnlyList<Book>, IReadOnlyList<GetCustomerBooksDto>> getCustomerBooksDto
    )
    {
        _getCustomerBooksDto = getCustomerBooksDto;
    }

    public GetCustomerDto Map(Customer customer)
    {
        return new GetCustomerDto(
            customer.Id,
            customer.FullName,
            customer.Email,
            customer.PhoneNumber,
            _getCustomerBooksDto.Map(customer.Books),
            customer.CustomerStatus,
            customer.CustomerPoints
        );
    }
}
