namespace BookRentalManager.Application.Customers.CommandHandlers;

internal sealed class ChangeCustomerBooksByBookIdsCommandHandler(IRepository<Customer> customerRepository, IRepository<Book> bookRepository)
    : IRequestHandler<ChangeCustomerBooksByBookIdsCommand>
{
    public async Task<Result> HandleAsync(
        ChangeCustomerBooksByBookIdsCommand changeCustomerBooksByBookIdsCommand,
        CancellationToken cancellationToken)
    {
        var customerByIdWithBooksSpecification = new CustomerByIdWithBooksSpecification(changeCustomerBooksByBookIdsCommand.Id);
        Customer? customer = await customerRepository.GetFirstOrDefaultBySpecificationAsync(customerByIdWithBooksSpecification, cancellationToken);
        if (customer is null)
        {
            return Result.Fail(RequestErrors.IdNotFoundError, $"No customer with the ID of '{changeCustomerBooksByBookIdsCommand.Id}' was found.");
        }
        var changeCustomerBooksByBookIdsDto = new ChangeCustomerBooksByBookIdsDto([]);
        Result patchAppliedResult = changeCustomerBooksByBookIdsCommand.ChangeCustomerBooksByBookIdsDtoPatchDocument.ApplyTo(
            changeCustomerBooksByBookIdsDto,
            ["replace", "remove"]);
        if (!patchAppliedResult.IsSuccess)
        {
            return patchAppliedResult;
        }
        IReadOnlyList<Book> books = await bookRepository.GetAllBySpecificationAsync(
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
                continue;
            }
            returnBookResults = Result.Combine(returnBookResults, customer.RentBook(book));
        }
        if (!returnBookResults.IsSuccess)
        {
            return returnBookResults;
        }
        await customerRepository.UpdateAsync(customer, cancellationToken);
        return Result.Success();
    }
}
