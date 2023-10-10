﻿// <auto-generated />
using System;
using BookRentalManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookRentalManager.Infrastructure.Data.Migrations
{
    [DbContext(typeof(BookRentalManagerDbContext))]
    [Migration("20230305222154_AddedInitialTables")]
    partial class AddedInitialTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AuthorBook", b =>
                {
                    b.Property<Guid>("AuthorsId")
                        .HasColumnType("uuid")
                        .HasColumnName("AuthorId");

                    b.Property<Guid>("BooksId")
                        .HasColumnType("uuid")
                        .HasColumnName("BookId");

                    b.HasKey("AuthorsId", "BooksId");

                    b.HasIndex("BooksId");

                    b.ToTable("Author_Book", (string)null);
                });

            modelBuilder.Entity("BookRentalManager.Domain.Entities.Author", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.HasKey("Id");

                    b.ToTable("Author", (string)null);
                });

            modelBuilder.Entity("BookRentalManager.Domain.Entities.Book", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("BookTitle")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("BookTitle");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<Guid?>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DueDate");

                    b.Property<DateTime?>("RentedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("RentedAt");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Book", (string)null);
                });

            modelBuilder.Entity("BookRentalManager.Domain.Entities.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<int>("CustomerPoints")
                        .HasColumnType("integer")
                        .HasColumnName("CustomerPoints");

                    b.HasKey("Id");

                    b.ToTable("Customer", (string)null);
                });

            modelBuilder.Entity("AuthorBook", b =>
                {
                    b.HasOne("BookRentalManager.Domain.Entities.Author", null)
                        .WithMany()
                        .HasForeignKey("AuthorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookRentalManager.Domain.Entities.Book", null)
                        .WithMany()
                        .HasForeignKey("BooksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookRentalManager.Domain.Entities.Author", b =>
                {
                    b.OwnsOne("BookRentalManager.Domain.ValueObjects.FullName", "FullName", b1 =>
                        {
                            b1.Property<Guid>("AuthorId")
                                .HasColumnType("uuid");

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("FirstName");

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("LastName");

                            b1.HasKey("AuthorId");

                            b1.ToTable("Author");

                            b1.WithOwner()
                                .HasForeignKey("AuthorId");
                        });

                    b.Navigation("FullName")
                        .IsRequired();
                });

            modelBuilder.Entity("BookRentalManager.Domain.Entities.Book", b =>
                {
                    b.HasOne("BookRentalManager.Domain.Entities.Customer", "Customer")
                        .WithMany("Books")
                        .HasForeignKey("CustomerId");

                    b.OwnsOne("BookRentalManager.Domain.ValueObjects.Edition", "Edition", b1 =>
                        {
                            b1.Property<Guid>("BookId")
                                .HasColumnType("uuid");

                            b1.Property<int>("EditionNumber")
                                .HasColumnType("integer")
                                .HasColumnName("Edition");

                            b1.HasKey("BookId");

                            b1.ToTable("Book");

                            b1.WithOwner()
                                .HasForeignKey("BookId");
                        });

                    b.OwnsOne("BookRentalManager.Domain.ValueObjects.Isbn", "Isbn", b1 =>
                        {
                            b1.Property<Guid>("BookId")
                                .HasColumnType("uuid");

                            b1.Property<string>("IsbnValue")
                                .IsRequired()
                                .HasMaxLength(20)
                                .HasColumnType("character varying(20)")
                                .HasColumnName("Isbn");

                            b1.HasKey("BookId");

                            b1.ToTable("Book");

                            b1.WithOwner()
                                .HasForeignKey("BookId");
                        });

                    b.Navigation("Customer");

                    b.Navigation("Edition")
                        .IsRequired();

                    b.Navigation("Isbn")
                        .IsRequired();
                });

            modelBuilder.Entity("BookRentalManager.Domain.Entities.Customer", b =>
                {
                    b.OwnsOne("BookRentalManager.Domain.ValueObjects.FullName", "FullName", b1 =>
                        {
                            b1.Property<Guid>("CustomerId")
                                .HasColumnType("uuid");

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("FirstName");

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("LastName");

                            b1.HasKey("CustomerId");

                            b1.ToTable("Customer");

                            b1.WithOwner()
                                .HasForeignKey("CustomerId");
                        });

                    b.OwnsOne("BookRentalManager.Domain.ValueObjects.CustomerStatus", "CustomerStatus", b1 =>
                        {
                            b1.Property<Guid>("CustomerId")
                                .HasColumnType("uuid");

                            b1.Property<string>("CustomerType")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("CustomerStatus");

                            b1.HasKey("CustomerId");

                            b1.ToTable("Customer");

                            b1.WithOwner()
                                .HasForeignKey("CustomerId");
                        });

                    b.OwnsOne("BookRentalManager.Domain.ValueObjects.Email", "Email", b1 =>
                        {
                            b1.Property<Guid>("CustomerId")
                                .HasColumnType("uuid");

                            b1.Property<string>("EmailAddress")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("Email");

                            b1.HasKey("CustomerId");

                            b1.ToTable("Customer");

                            b1.WithOwner()
                                .HasForeignKey("CustomerId");
                        });

                    b.OwnsOne("BookRentalManager.Domain.ValueObjects.PhoneNumber", "PhoneNumber", b1 =>
                        {
                            b1.Property<Guid>("CustomerId")
                                .HasColumnType("uuid");

                            b1.Property<int>("AreaCode")
                                .HasMaxLength(12)
                                .HasColumnType("integer")
                                .HasColumnName("AreaCode")
                                .IsFixedLength();

                            b1.Property<int>("PrefixAndLineNumber")
                                .HasMaxLength(12)
                                .HasColumnType("integer")
                                .HasColumnName("PrefixAndLineNumber")
                                .IsFixedLength();

                            b1.HasKey("CustomerId");

                            b1.ToTable("Customer");

                            b1.WithOwner()
                                .HasForeignKey("CustomerId");
                        });

                    b.Navigation("CustomerStatus")
                        .IsRequired();

                    b.Navigation("Email")
                        .IsRequired();

                    b.Navigation("FullName")
                        .IsRequired();

                    b.Navigation("PhoneNumber")
                        .IsRequired();
                });

            modelBuilder.Entity("BookRentalManager.Domain.Entities.Customer", b =>
                {
                    b.Navigation("Books");
                });
#pragma warning restore 612, 618
        }
    }
}
