namespace BookRentalManager.Infrastructure.Data.Config;

public sealed class BookEntityBuilder : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> bookBuilder)
    {
        bookBuilder
            .ToTable("Book")
            .HasKey(book => book.Id);
        bookBuilder
            .Property(book => book.BookTitle)
            .HasColumnName("BookTitle")
            .IsRequired();
        bookBuilder
            .HasMany(book => book.BookAuthors)
            .WithMany(bookAuthors => bookAuthors.Books)
            .UsingEntity(bookBuilder =>
            {
                bookBuilder
                    .ToTable("BookAuthor_Book")
                    .Property("BooksId")
                    .HasColumnName("BookId");
            });
        bookBuilder
            .OwnsOne(book => book.Edition)
            .Property(edition => edition.EditionNumber)
            .HasColumnName("Edition")
            .IsRequired();
        bookBuilder
            .OwnsOne(book => book.Isbn)
            .Property(isbn => isbn.IsbnValue)
            .HasColumnName("Isbn")
            .HasMaxLength(13)
            .IsRequired();
        bookBuilder
            .Property(book => book.IsAvailable)
            .HasColumnName("IsAvailable")
            .IsRequired();
        bookBuilder
            .HasOne(book => book.Customer)
            .WithMany(customer => customer.Books);
    }
}
