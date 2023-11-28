FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app/
EXPOSE 80
EXPOSE 443
COPY ./src/BookRentalManager.Domain/BookRentalManager.Domain.csproj ./src/BookRentalManager.Domain/BookRentalManager.Domain.csproj
COPY ./src/BookRentalManager.Application/BookRentalManager.Application.csproj ./src/BookRentalManager.Application/BookRentalManager.Application.csproj
COPY ./src/BookRentalManager.Infrastructure/BookRentalManager.Infrastructure.csproj ./src/BookRentalManager.Infrastructure/BookRentalManager.Infrastructure.csproj
COPY ./src/BookRentalManager.Api/BookRentalManager.Api.csproj ./src/BookRentalManager.Api/BookRentalManager.Api.csproj
RUN ["dotnet", "restore", "./src/BookRentalManager.Domain/BookRentalManager.Domain.csproj"]
RUN ["dotnet", "restore", "./src/BookRentalManager.Application/BookRentalManager.Application.csproj"]
RUN ["dotnet", "restore", "./src/BookRentalManager.Infrastructure/BookRentalManager.Infrastructure.csproj"]
RUN ["dotnet", "restore", "./src/BookRentalManager.Api/BookRentalManager.Api.csproj"]
COPY ./src/BookRentalManager.Domain/ ./src/BookRentalManager.Domain/
COPY ./src/BookRentalManager.Application/ ./src/BookRentalManager.Application/
COPY ./src/BookRentalManager.Infrastructure/ ./src/BookRentalManager.Infrastructure/
COPY ./src/BookRentalManager.Api/ ./src/BookRentalManager.Api/
WORKDIR /app/src/BookRentalManager.Api/
RUN ["dotnet", "publish", "BookRentalManager.Api.csproj", "-c", "Release", "-o", "/app/release/", "--no-restore"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app/
COPY ./httpsdevcert.pfx ./httpsdevcert.pfx
COPY --from=build /app/release/ ./release/
WORKDIR /app/release/
ENTRYPOINT ["dotnet", "BookRentalManager.Api.dll"]
