namespace BookRentalManager.Domain.Specifications;

public sealed class BookByTitleEditionAndIsbnSpecification : Specification<Book>
{
    public BookByTitleEditionAndIsbnSpecification(string bookTitle, int edition, string isbn)
    {
        Where = book => book.BookTitle == bookTitle
            && book.Edition.EditionNumber == edition
            && book.Isbn.IsbnValue == isbn;
    }
}
