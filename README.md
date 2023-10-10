# book-rental-manager
The backend of a book rental management system created with .NET, PostgreSQL, Swagger and clean architecture with CQRS.

## Running the application
`dotnet watch run` will open up the application on http://localhost:5007/swagger/index.html.

## Add migrations
`dotnet ef migrations add <MigrationName> -s .\src\BookRentalManager.Api\ -p .\src\BookRentalManager.Infrastructure\ -o Data\Migrations\`.

## Remove migrations
`dotnet ef migrations remove -s .\src\BookRentalManager.Api\ -p .\src\BookRentalManager.Infrastructure\`.

## Update database
`dotnet ef database update -s .\src\BookRentalManager.Api\ -p .\src\BookRentalManager.Infrastructure\`.

## PostgreSQL
After installing PostgreSQL on your machine:
* Windows users:
    1. Put the directory `C:\Program Files\PostgreSQL\15\bin` in the `PATH` variable;
    2. Run `psql -U postgres` and type in the password set up during the PostgreSQL installation;
    3. Run `CREATE ROLE admin WITH CREATEDB LOGIN PASSWORD 'admin';`.
* Linux users:
    1. Run `sudo su - postgres`;
    2. Run `psql`;
    3. Run `CREATE ROLE admin WITH CREATEDB LOGIN PASSWORD 'admin';`.