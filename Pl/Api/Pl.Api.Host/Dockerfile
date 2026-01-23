# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Pl/Api/Pl.Api.Host/Pl.Api.Host.csproj", "Pl/Api/Pl.Api.Host/"]
COPY ["Bl/Bl.DependencyInjection/Bl.DependencyInjection.csproj", "Bl/Bl.DependencyInjection/"]
COPY ["Bl/Bl.Implementations/Bl.Implementations.csproj", "Bl/Bl.Implementations/"]
COPY ["Common/Common.Helpers/Common.Helpers.csproj", "Common/Common.Helpers/"]
COPY ["Common/Common.Constants/Common.Constants.csproj", "Common/Common.Constants/"]
COPY ["Dal/Dal.Interfaces/Dal.Interfaces.csproj", "Dal/Dal.Interfaces/"]
COPY ["Common/Common.ConvertParams/Common.ConvertParams.csproj", "Common/Common.ConvertParams/"]
COPY ["Common/Common.SearchParams/Common.SearchParams.csproj", "Common/Common.SearchParams/"]
COPY ["Common/Common.Enums/Common.Enums.csproj", "Common/Common.Enums/"]
COPY ["Domain/Entities/Entities.csproj", "Domain/Entities/"]
COPY ["Dal/S3/Dal.S3.Interfaces/Dal.S3.Interfaces.csproj", "Dal/S3/Dal.S3.Interfaces/"]
COPY ["Common/Common.Storage.Dtos/Common.Storage.Dtos.csproj", "Common/Common.Storage.Dtos/"]
COPY ["Domain/Dto/Dto.csproj", "Domain/Dto/"]
COPY ["Bl/Bl.Interfaces/Bl.Interfaces.csproj", "Bl/Bl.Interfaces/"]
COPY ["Dal/Dal.DependencyInjection/Dal.DependencyInjection.csproj", "Dal/Dal.DependencyInjection/"]
COPY ["Dal/Ef/Dal.Ef.DependencyInjection/Dal.Ef.DependencyInjection.csproj", "Dal/Ef/Dal.Ef.DependencyInjection/"]
COPY ["Dal/Ef/Dal.Ef.Implementations/Dal.Ef.Implementations.csproj", "Dal/Ef/Dal.Ef.Implementations/"]
COPY ["Common/Common.Exceptions/Common.Exceptions.csproj", "Common/Common.Exceptions/"]
COPY ["Dal/Ef/Dal.Ef.DbModels/Dal.Ef.DbModels.csproj", "Dal/Ef/Dal.Ef.DbModels/"]
COPY ["Common/Common.Attributes/Common.Attributes.csproj", "Common/Common.Attributes/"]
COPY ["Dal/Ef/Dal.Ef.Filters/Dal.Ef.Filters.csproj", "Dal/Ef/Dal.Ef.Filters/"]
COPY ["Dal/Ef/Dal.Ef.Includes/Dal.Ef.Includes.csproj", "Dal/Ef/Dal.Ef.Includes/"]
COPY ["Dal/Ef/Dal.Ef.Mappers/Dal.Ef.Mappers.csproj", "Dal/Ef/Dal.Ef.Mappers/"]
COPY ["Dal/Ef/Dal.Ef/Dal.Ef.csproj", "Dal/Ef/Dal.Ef/"]
COPY ["Dal/MongoDb/Dal.MongoDb.DependencyInjection/Dal.MongoDb.DependencyInjection.csproj", "Dal/MongoDb/Dal.MongoDb.DependencyInjection/"]
COPY ["Dal/MongoDb/Dal.MongoDb.Implementations/Dal.MongoDb.Implementations.csproj", "Dal/MongoDb/Dal.MongoDb.Implementations/"]
COPY ["Dal/MongoDb/Dal.MongoDb.DbModels/Dal.MongoDb.DbModels.csproj", "Dal/MongoDb/Dal.MongoDb.DbModels/"]
COPY ["Dal/MongoDb/Dal.MongoDb.Filters/Dal.MongoDb.Filters.csproj", "Dal/MongoDb/Dal.MongoDb.Filters/"]
COPY ["Dal/MongoDb/Dal.MongoDb.Mappers/Dal.MongoDb.Mappers.csproj", "Dal/MongoDb/Dal.MongoDb.Mappers/"]
COPY ["Dal/S3/Dal.S3.DependencyInjection/Dal.S3.DependencyInjection.csproj", "Dal/S3/Dal.S3.DependencyInjection/"]
COPY ["Dal/S3/Dal.S3.Implementations/Dal.S3.Implementations.csproj", "Dal/S3/Dal.S3.Implementations/"]
COPY ["Dal/S3/Dal.S3.Configurations/Dal.S3.Configurations.csproj", "Dal/S3/Dal.S3.Configurations/"]
COPY ["Pl/Api/Http/Pl.Api.Http.Composition/Pl.Api.Http.Composition.csproj", "Pl/Api/Http/Pl.Api.Http.Composition/"]
COPY ["Pl/Api/Http/Pl.Api.Http.Controllers/Pl.Api.Http.Controllers.csproj", "Pl/Api/Http/Pl.Api.Http.Controllers/"]
COPY ["Pl/Api/Http/Pl.Api.Http.Dtos/Pl.Api.Http.Dtos.csproj", "Pl/Api/Http/Pl.Api.Http.Dtos/"]
COPY ["Pl/Api/Http/Pl.Api.Http.Mappers/Pl.Api.Http.Mappers.csproj", "Pl/Api/Http/Pl.Api.Http.Mappers/"]
COPY ["Pl/Api/Http/Pl.Api.Http.Middlewares/Pl.Api.Http.Middlewares.csproj", "Pl/Api/Http/Pl.Api.Http.Middlewares/"]
RUN dotnet restore "./Pl/Api/Pl.Api.Host/Pl.Api.Host.csproj"
COPY . .
WORKDIR "/src/Pl/Api/Pl.Api.Host"
RUN dotnet build "./Pl.Api.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Pl.Api.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pl.Api.Host.dll"]