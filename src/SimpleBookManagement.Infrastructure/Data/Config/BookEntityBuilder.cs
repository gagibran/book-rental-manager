namespace SimpleBookManagement.Infrastructure.Data.Config;

public sealed class BookEntityBuilder : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> bookBuilder)
    {
        bookBuilder.ToTable("Book");
        bookBuilder.HasKey(book => book.Id);
        bookBuilder
            .Property(book => book.BookTitle)
            .HasColumnName("BookTitle")
            .IsRequired();
        bookBuilder
            .OwnsMany(book => book.BookAuthors)
            .Property(authors => authors.CompleteName)
            .HasColumnName("AuthorName")
            .IsRequired();
        bookBuilder
            .OwnsOne(book => book.Volume)
            .Property(volume => volume.VolumeNumber)
            .HasColumnName("Volume")
            .IsRequired();
        bookBuilder
            .OwnsOne(book => book.Isbn)
            .Property(isbn => isbn.IsbnNumber)
            .HasColumnName("Isbn")
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
