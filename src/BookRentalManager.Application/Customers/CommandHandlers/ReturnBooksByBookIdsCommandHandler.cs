namespace BookRentalManager.Application.Customers.CommandHandlers;

internal sealed class ReturnBooksByBookIdsCommandHandler : ICommandHandler<ReturnBooksByBookIdsCommand>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Book> _bookRepository;

    public ReturnBooksByBookIdsCommandHandler(IRepository<Customer> customerRepository, IRepository<Book> bookRepository)
    {
        _customerRepository = customerRepository;
        _bookRepository = bookRepository;
    }

    public async Task<Result> HandleAsync(ReturnBooksByBookIdsCommand returnBooksByBookIdsCommand, CancellationToken cancellationToken)
    {
        var customerByIdWithBooksSpecification = new CustomerByIdWithBooksSpecification(returnBooksByBookIdsCommand.Id);
        Customer? customer = await _customerRepository.GetFirstOrDefaultBySpecificationAsync(customerByIdWithBooksSpecification, cancellationToken);
        if (customer is null)
        {
            return Result.Fail("customerId", $"No customer with the ID of '{returnBooksByBookIdsCommand.Id}' was found.");
        }
        var returnCustomerBookByIdDto = new ReturnCustomerBookByIdDto(new List<Guid>());
        Result patchAppliedResult = returnBooksByBookIdsCommand.ReturnCustomerBookByIdDtoPatchDocument.ApplyTo(
            returnCustomerBookByIdDto,
            new string[] { "replace", "remove" });
        if (!patchAppliedResult.IsSuccess)
        {
            return patchAppliedResult;
        }
        IReadOnlyList<Book> books = await _bookRepository.GetAllBySpecificationAsync(new BooksByIdsSpecification(returnCustomerBookByIdDto.BookIds));
        if (books.Count() != returnCustomerBookByIdDto.BookIds.Count())
        {
            return Result.Fail("bookIds", "Could not find some of the books for the provided IDs.");
        }
        Result returnBookResults = Result.Success();
        foreach (Book book in books)
        {
            returnBookResults = Result.Combine(returnBookResults, customer.ReturnBook(book));
        }
        if (!returnBookResults.IsSuccess)
        {
            return returnBookResults;
        }
        await _customerRepository.UpdateAsync(customer);
        return Result.Success();
    }
}
