# book-rental-manager
A book rental management system created with .NET, Angular, Docker, PostgreSQL and the CQRS architecture.

## Add Migrations
`dotnet ef migrations add <MigrationName> -s .\src\BookRentalManager.Api\ -p .\src\BookRentalManager.Infrastructure\ -o Data\Migrations\`.

## Update Database
`dotnet ef database update -s .\src\BookRentalManager.Api\ -p .\src\BookRentalManager.Infrastructure\`.
