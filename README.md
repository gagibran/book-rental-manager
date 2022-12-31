# simple-book-management
A template project using .NET, Angular, Docker, PostgreSQL and the CQRS architecture.

## Add Migrations
`dotnet ef migrations add AddedCustomerAndBook -s .\src\SimpleBookManagement.Api\ -p .\src\SimpleBookManagement.Infrastructure\ -o Data\Migrations\`.

## Update Database
`dotnet ef database update -s .\src\SimpleBookManagement.Api\ -p .\src\SimpleBookManagement.Infrastructure\`.
