# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Dal/Ef/Dal.Ef.Migrator/Dal.Ef.Migrator.csproj", "Dal/Ef/Dal.Ef.Migrator/"]
COPY ["Dal/Ef/Dal.Ef.DependencyInjection/Dal.Ef.DependencyInjection.csproj", "Dal/Ef/Dal.Ef.DependencyInjection/"]
COPY ["Dal/Dal.Interfaces/Dal.Interfaces.csproj", "Dal/Dal.Interfaces/"]
COPY ["Common/Common.ConvertParams/Common.ConvertParams.csproj", "Common/Common.ConvertParams/"]
COPY ["Common/Common.SearchParams/Common.SearchParams.csproj", "Common/Common.SearchParams/"]
COPY ["Common/Common.Enums/Common.Enums.csproj", "Common/Common.Enums/"]
COPY ["Domain/Entities/Entities.csproj", "Domain/Entities/"]
COPY ["Dal/Ef/Dal.Ef.Implementations/Dal.Ef.Implementations.csproj", "Dal/Ef/Dal.Ef.Implementations/"]
COPY ["Common/Common.Exceptions/Common.Exceptions.csproj", "Common/Common.Exceptions/"]
COPY ["Dal/Ef/Dal.Ef.DbModels/Dal.Ef.DbModels.csproj", "Dal/Ef/Dal.Ef.DbModels/"]
COPY ["Common/Common.Attributes/Common.Attributes.csproj", "Common/Common.Attributes/"]
COPY ["Dal/Ef/Dal.Ef.Filters/Dal.Ef.Filters.csproj", "Dal/Ef/Dal.Ef.Filters/"]
COPY ["Dal/Ef/Dal.Ef.Includes/Dal.Ef.Includes.csproj", "Dal/Ef/Dal.Ef.Includes/"]
COPY ["Dal/Ef/Dal.Ef.Mappers/Dal.Ef.Mappers.csproj", "Dal/Ef/Dal.Ef.Mappers/"]
COPY ["Dal/Ef/Dal.Ef/Dal.Ef.csproj", "Dal/Ef/Dal.Ef/"]
COPY ["Common/Common.Constants/Common.Constants.csproj", "Common/Common.Constants/"]
RUN dotnet restore "./Dal/Ef/Dal.Ef.Migrator/Dal.Ef.Migrator.csproj"
COPY . .
WORKDIR "/src/Dal/Ef/Dal.Ef.Migrator"
RUN dotnet build "./Dal.Ef.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Dal.Ef.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dal.Ef.Migrator.dll"]