[![Build And Test](https://github.com/gagibran/book-rental-manager/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/gagibran/book-rental-manager/actions/workflows/build-and-test.yml)

# Book Rental Manager
The backend of a fake book rental management system.

## Topics
- [Description](#description)
- [Disclaimer](#disclaimer)
- [Running the application](#running-the-application)
    - [Running it with Docker](#running-it-with-docker)
    - [Running it locally](#running-it-locally)
    - [Debugging with Docker](#debugging-with-docker)
    - [Running it in production](#running-it-in-production)
- [Migrations](#migrations)
    - [Creating a migration](#creating-a-migration)
    - [Removing a migration](#removing-a-migration)
    - [Updating the database after creating or removing a migration](#updating-the-database-after-creating-or-removing-a-migration)
- [Testing](#testing)

## Description
This project was created with .NET, PostgreSQL, Swagger and Docker, using techniques like clean architecture with CQRS and some domain-driven design concepts. I am also using HATEOAS in order to improve API discoverability. The goal of this project is to practice these technologies and techniques. I have not implemented authentication nor the frontend for this project at this time.

## Disclaimer
This system is not obviously big nor that complex, so, in a real world scenario, I would probably choose a different architecture to implement this. My choice would be based upon the requirements, deadlines and team size. The only reason I used the architecture that's in place was for learning purposes. I would also not go as far as implementing the methods in `BookRentalManager.Domain/IQueryableExtensions.cs` just for the sake of not having any infrastructure dependencies in the domain layer. I just did that to showcase what would need to be done in order to have them completely decoupled.

## Running the application

### Running it with Docker
You need to have [Docker](https://docs.docker.com/engine/) and [Docker Compose](https://docs.docker.com/compose/) installed in order to make it work.

Generate a PFX certificate so the application can be run using HTTPS inside the Docker container.
Go to the repository's root folder and execute the following commands:
```sh
dotnet dev-certs https -ep httpsdevcert.pfx -p HttpsDevCertPass1!`
dotnet dev-certs https --trust
```

Create a `.env` file in the repository's root folder with:
```env
WITH_INITIAL_DATA=TRUE
```
In it. If you want the database to be create with some starting data. This will build the Docker image with:
```dockerfile
ENTRYPOINT dotnet BookRentalManager.Api.dll --with-initial-data=true
```
And run:
```sh
docker compose up -d
```
In order to spin up both database and application services with a named volume in detach mode. The database utilizes a PostgreSQL 16 image and the application is being built by the `Dockerfile` present in the root of this repository. If you want to spin it up with a clean database, put:
```env
WITH_INITIAL_DATA=FALSE
```
In the `.env` file.

After starting everything, the application will be available at https://localhost:5001/index.html and http://localhost:5000/index.html, although HTTPS redirection is turned on. It's a Swagger page.

If you want stop both services and completely delete the named volume, run:
```sh
docker compose down -v
```
Check the `docker-compose.yml` file, the `Dockerfile` and the `scripts/entrypoint.sh` for more information. Also check `src/BookRentalManager.Infrastructure/Data/Seeds/TestDataSeeder.cs` to understand what data will be seeded.

### Running it locally
You will need [.NET 8.0.x](https://dotnet.microsoft.com/en-us/download) and [PostgreSQL 16.0](https://www.postgresql.org/download/) installed. Firstly, you will need to create an administrator user on PostgreSQL local instance. This is required in order to run the application. After installing PostgreSQL on your machine:
* Windows users:
    1. Put the directory `C:\Program Files\PostgreSQL\15\bin` in the `PATH` variable;
    2. Run `psql -U postgres` and type in the password set up during the PostgreSQL installation;
    3. Run `CREATE ROLE admin WITH CREATEDB LOGIN PASSWORD 'admin';`.
* Linux users:
    1. Run `sudo su - postgres`;
    2. Run `psql`;
    3. Run `CREATE ROLE admin WITH CREATEDB LOGIN PASSWORD 'admin';`.

Finally, run:
```sh
cd src/BookRentalManager.Api
dotnet run
```
Or
```sh
cd src/BookRentalManager.Api
dotnet run --with-initial-data=true
```
If you want the database to be create with some starting data. Running either of these commands will start the application on https://localhost:5001/index.html  and http://localhost:5000/index.html, although HTTPS redirection is turned on. It's a Swagger page. Check `src/BookRentalManager.Infrastructure/Data/Seeds/TestDataSeeder.cs` to understand what data will be seeded.

### Debugging with Docker
You can also run:
```sh
docker compose database -d
```
To spin up only the database and debug the application using your IDE.

### Running it in production
If you choose to change the environment to production, the database will not be automatically migrated, nor the option to run it with `--with-initial-data=true` to seed some initial data will be available. This means you will need to manually [migrate](#migrations) everything

## Migrations
In case someone wants to fork or expand on this project, this is how I set up migrations.

We'll need [Entity Framework Core tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) installed for this.

### Creating a migration
Run the following command to create one, replacing `<MigrationName>` with its actual name:
```sh
dotnet ef migrations add <MigrationName> -s .\src\BookRentalManager.Api\ -p .\src\BookRentalManager.Infrastructure\ -o Data\Migrations\
```

### Removing a migration
In order to undo a recently created migration, run:
```sh
dotnet ef migrations remove -s .\src\BookRentalManager.Api\ -p .\src\BookRentalManager.Infrastructure\
```

### Updating the database after creating or removing a migration
Finally, update the database:
```sh
dotnet ef database update -s .\src\BookRentalManager.Api\ -p .\src\BookRentalManager.Infrastructure\
```

## Testing
The `tests` folder contains both unit and integration tests. You will need Docker installed and running in order to run the integration tests, since they use [Testcontainers](https://dotnet.testcontainers.org/). Run:
```sh
dotnet test
```
To run all of the tests at once.