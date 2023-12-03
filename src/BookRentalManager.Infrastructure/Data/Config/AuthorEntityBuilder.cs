namespace BookRentalManager.Infrastructure.Data.Config;

public sealed class AuthorEntityBuilder : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> authorBuilder)
    {
        authorBuilder
            .ToTable("Author")
            .HasKey(author => author.Id);
        authorBuilder
            .OwnsOne(author => author.FullName)
            .Property(fullName => fullName.FirstName)
            .HasColumnName("FirstName")
            .IsRequired();
        authorBuilder
            .OwnsOne(author => author.FullName)
            .Property(fullName => fullName.LastName)
            .HasColumnName("LastName")
            .IsRequired();
        authorBuilder
            .HasMany(author => author.Books)
            .WithMany(book => book.Authors)
            .UsingEntity(authorBuilder =>
            {
                authorBuilder
                    .ToTable("Author_Book")
                    .Property("AuthorsId")
                    .HasColumnName("AuthorId");
            });
        authorBuilder
            .Property(author => author.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();
    }
}
