# book-rental-manager
A book rental management system created with .NET, Angular, Docker, PostgreSQL and the CQRS architecture.

## Add Migrations
`dotnet ef migrations add <MigrationName> -s .\src\BookRentalManager.Api\ -p .\src\BookRentalManager.Infrastructure\ -o Data\Migrations\`.

## Remove Migrations
`dotnet ef migrations remove -s .\src\BookRentalManager.Api\ -p .\src\BookRentalManager.Infrastructure\`.

## Update Database
`dotnet ef database update -s .\src\BookRentalManager.Api\ -p .\src\BookRentalManager.Infrastructure\`.

## PostgreSQL
1. Put the directory `C:\Program Files\PostgreSQL\15\bin` in the `PATH` variable;
2. Run `psql -U postgres` and type in the password set up during the PostgreSQL installation;
3. Run `CREATE ROLE admin WITH LOGIN SUPERUSER PASSWORD 'admin';`.