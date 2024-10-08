﻿namespace BookRentalManager.IntegrationTests.Common;

public sealed record AuthorWithHateoasLinks(string FullName, Guid Id, List<GetBookDto> Books, List<HateoasLinkDto> Links);
public sealed record AuthorsWithHateoasLinks(List<AuthorWithHateoasLinks> Values, List<HateoasLinkDto> Links);
public sealed record BookWithHateoasLinks(
    string BookTitle,
    Guid Id,
    List<GetAuthorFromBookDto> Authors,
    int Edition,
    string Isbn,
    DateTime? RentedAt,
    DateTime? DueDate,
    GetCustomerThatRentedBookDto? RentedBy,
    List<HateoasLinkDto> Links);
public sealed record BooksWithHateoasLinks(List<BookWithHateoasLinks> Values, List<HateoasLinkDto> Links);
public sealed record CustomerWithHateoasLinks(
    string FullName,
    Guid Id,
    string Email,
    string PhoneNumber,
    IReadOnlyList<GetBookRentedByCustomerDto> Books,
    string CustomerStatus,
    int CustomerPoints,
    List<HateoasLinkDto> Links);
public sealed record CustomersWithHateoasLinks(List<CustomerWithHateoasLinks> Values, List<HateoasLinkDto> Links);
