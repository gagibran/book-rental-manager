namespace BookRentalManager.Infrastructure.Data.Configurations;

public sealed class BookEntityBuilder : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> bookBuilder)
    {
        bookBuilder
            .ToTable("Book")
            .HasKey(book => book.Id);
        bookBuilder
            .OwnsOne(book => book.BookTitle)
            .Property(bookTitle => bookTitle.Title)
            .HasColumnName("BookTitle")
            .IsRequired();
        bookBuilder
            .HasMany(book => book.Authors)
            .WithMany(author => author.Books)
            .UsingEntity(bookBuilder =>
            {
                bookBuilder
                    .ToTable("Author_Book")
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
            .HasMaxLength(20)
            .IsRequired();
        bookBuilder
            .Property(book => book.RentedAt)
            .HasColumnName("RentedAt");
        bookBuilder
            .Property(book => book.DueDate)
            .HasColumnName("DueDate");
        bookBuilder
            .HasOne(book => book.Customer)
            .WithMany(customer => customer.Books);
        bookBuilder
            .Property(books => books.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();
    }
}
