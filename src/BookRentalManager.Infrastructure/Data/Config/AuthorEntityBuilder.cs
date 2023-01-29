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
            .Property(fullName => fullName.CompleteName)
            .HasColumnName("FullName")
            .IsRequired();
        authorBuilder
            .HasMany(author => author.Books)
            .WithMany(books => books.Authors)
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
