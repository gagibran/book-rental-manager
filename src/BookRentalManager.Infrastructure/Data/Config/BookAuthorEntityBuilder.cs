namespace BookRentalManager.Infrastructure.Data.Config;

public sealed class BookAuthorEntityBuilder : IEntityTypeConfiguration<BookAuthor>
{
    public void Configure(EntityTypeBuilder<BookAuthor> bookAuthorBuilder)
    {
        bookAuthorBuilder
            .ToTable("BookAuthor")
            .HasKey(bookAuthor => bookAuthor.Id);
        bookAuthorBuilder
            .OwnsOne(bookAuthor => bookAuthor.FullName)
            .Property(fullName => fullName.CompleteName)
            .HasColumnName("FullName")
            .IsRequired();
        bookAuthorBuilder
            .HasMany(bookAuthor => bookAuthor.Books)
            .WithMany(books => books.BookAuthors)
            .UsingEntity(bookAuthorBuilder =>
            {
                bookAuthorBuilder
                    .ToTable("Book_BookAuthor")
                    .Property("BookAuthorsId")
                    .HasColumnName("BookAuthorId");
            });
    }
}
