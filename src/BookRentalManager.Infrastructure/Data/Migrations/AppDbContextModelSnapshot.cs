﻿// <auto-generated />
using System;
using BookRentalManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookRentalManager.Infrastructure.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BookBookAuthor", b =>
                {
                    b.Property<Guid>("BookAuthorsId")
                        .HasColumnType("uuid")
                        .HasColumnName("BookAuthorId");

                    b.Property<Guid>("BooksId")
                        .HasColumnType("uuid")
                        .HasColumnName("BookId");

                    b.HasKey("BookAuthorsId", "BooksId");

                    b.HasIndex("BooksId");

                    b.ToTable("Book_BookAuthor", (string)null);
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

                    b.Property<Guid?>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("boolean")
                        .HasColumnName("IsAvailable");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Book", (string)null);
                });

            modelBuilder.Entity("BookRentalManager.Domain.Entities.BookAuthor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("BookAuthor", (string)null);
                });

            modelBuilder.Entity("BookRentalManager.Domain.Entities.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("CustomerPoints")
                        .HasColumnType("integer")
                        .HasColumnName("CustomerPoints");

                    b.HasKey("Id");

                    b.ToTable("Customer", (string)null);
                });

            modelBuilder.Entity("BookBookAuthor", b =>
                {
                    b.HasOne("BookRentalManager.Domain.Entities.BookAuthor", null)
                        .WithMany()
                        .HasForeignKey("BookAuthorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookRentalManager.Domain.Entities.Book", null)
                        .WithMany()
                        .HasForeignKey("BooksId")
                        .OnDelete(DeleteBehavior.Cascade)
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
                                .HasMaxLength(13)
                                .HasColumnType("character varying(13)")
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

            modelBuilder.Entity("BookRentalManager.Domain.Entities.BookAuthor", b =>
                {
                    b.OwnsOne("BookRentalManager.Domain.ValueObjects.FullName", "FullName", b1 =>
                        {
                            b1.Property<Guid>("BookAuthorId")
                                .HasColumnType("uuid");

                            b1.Property<string>("CompleteName")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("FullName");

                            b1.HasKey("BookAuthorId");

                            b1.ToTable("BookAuthor");

                            b1.WithOwner()
                                .HasForeignKey("BookAuthorId");
                        });

                    b.Navigation("FullName")
                        .IsRequired();
                });

            modelBuilder.Entity("BookRentalManager.Domain.Entities.Customer", b =>
                {
                    b.OwnsOne("BookRentalManager.Domain.ValueObjects.FullName", "FullName", b1 =>
                        {
                            b1.Property<Guid>("CustomerId")
                                .HasColumnType("uuid");

                            b1.Property<string>("CompleteName")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("FullName");

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

                            b1.Property<string>("CompletePhoneNumber")
                                .IsRequired()
                                .HasMaxLength(12)
                                .HasColumnType("character(12)")
                                .HasColumnName("PhoneNumber")
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
