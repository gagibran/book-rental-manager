namespace BookRentalManager.Application.Customers.CommandHandlers;

internal sealed class ChangeCustomerBooksByBookIdsCommandHandler : IRequestHandler<ChangeCustomerBooksByBookIdsCommand>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Book> _bookRepository;

    public ChangeCustomerBooksByBookIdsCommandHandler(IRepository<Customer> customerRepository, IRepository<Book> bookRepository)
    {
        _customerRepository = customerRepository;
        _bookRepository = bookRepository;
    }

    public async Task<Result> HandleAsync(
        ChangeCustomerBooksByBookIdsCommand changeCustomerBooksByBookIdsCommand,
        CancellationToken cancellationToken)
    {
        var customerByIdWithBooksSpecification = new CustomerByIdWithBooksSpecification(changeCustomerBooksByBookIdsCommand.Id);
        Customer? customer = await _customerRepository.GetFirstOrDefaultBySpecificationAsync(customerByIdWithBooksSpecification, cancellationToken);
        if (customer is null)
        {
            return Result.Fail("customerId", $"No customer with the ID of '{changeCustomerBooksByBookIdsCommand.Id}' was found.");
        }
        var changeCustomerBooksByBookIdsDto = new ChangeCustomerBooksByBookIdsDto(new List<Guid>());
        Result patchAppliedResult = changeCustomerBooksByBookIdsCommand.ChangeCustomerBooksByBookIdsDtoPatchDocument.ApplyTo(
            changeCustomerBooksByBookIdsDto,
            ["replace", "remove"]);
        if (!patchAppliedResult.IsSuccess)
        {
            return patchAppliedResult;
        }
        IReadOnlyList<Book> books = await _bookRepository.GetAllBySpecificationAsync(
            new BooksByIdsSpecification(changeCustomerBooksByBookIdsDto.BookIds),
            cancellationToken);
        if (books.Count != changeCustomerBooksByBookIdsDto.BookIds.Count())
        {
            return Result.Fail("bookIds", "Could not find some of the books for the provided IDs.");
        }
        Result returnBookResults = Result.Success();
        foreach (Book book in books)
        {
            if (changeCustomerBooksByBookIdsCommand.IsReturn)
            {
                returnBookResults = Result.Combine(returnBookResults, customer.ReturnBook(book));
            }
            else
            {
                returnBookResults = Result.Combine(returnBookResults, customer.RentBook(book));
            }
        }
        if (!returnBookResults.IsSuccess)
        {
            return returnBookResults;
        }
        await _customerRepository.UpdateAsync(customer, cancellationToken);
        return Result.Success();
    }
}
