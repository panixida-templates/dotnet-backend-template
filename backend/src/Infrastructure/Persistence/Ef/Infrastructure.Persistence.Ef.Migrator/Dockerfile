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
COPY ["backend/src/Infrastructure/Persistence/Ef/Infrastructure.Persistence.Ef.Migrator/Infrastructure.Persistence.Ef.Migrator.csproj", "backend/src/Infrastructure/Persistence/Ef/Infrastructure.Persistence.Ef.Migrator/"]
COPY ["backend/src/Infrastructure/Persistence/Ef/Infrastructure.Persistence.Ef/Infrastructure.Persistence.Ef.csproj", "backend/src/Infrastructure/Persistence/Ef/Infrastructure.Persistence.Ef/"]
COPY ["backend/src/Application/Abstractions/Application.Abstractions.Persistence/Application.Abstractions.Persistence.csproj", "backend/src/Application/Abstractions/Application.Abstractions.Persistence/"]
COPY ["backend/src/Common/Common.ConvertParams/Common.ConvertParams.csproj", "backend/src/Common/Common.ConvertParams/"]
COPY ["backend/src/Common/Common.SearchParams/Common.SearchParams.csproj", "backend/src/Common/Common.SearchParams/"]
COPY ["backend/src/Domain/Domain.Enums/Domain.Enums.csproj", "backend/src/Domain/Domain.Enums/"]
COPY ["backend/src/Domain/Domain.Entities/Domain.Entities.csproj", "backend/src/Domain/Domain.Entities/"]
COPY ["backend/src/Domain/Domain.Events/Domain.Events.csproj", "backend/src/Domain/Domain.Events/"]
COPY ["backend/src/Domain/Domain.ValueObjects/Domain.ValueObjects.csproj", "backend/src/Domain/Domain.ValueObjects/"]
COPY ["backend/src/Domain/Domain.Exceptions/Domain.Exceptions.csproj", "backend/src/Domain/Domain.Exceptions/"]
COPY ["backend/src/Common/Common.Constants/Common.Constants.csproj", "backend/src/Common/Common.Constants/"]
COPY ["backend/src/Common/Common.Exceptions/Common.Exceptions.csproj", "backend/src/Common/Common.Exceptions/"]
RUN dotnet restore "./backend/src/Infrastructure/Persistence/Ef/Infrastructure.Persistence.Ef.Migrator/Infrastructure.Persistence.Ef.Migrator.csproj"
COPY . .
WORKDIR "/src/backend/src/Infrastructure/Persistence/Ef/Infrastructure.Persistence.Ef.Migrator"
RUN dotnet build "./Infrastructure.Persistence.Ef.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Infrastructure.Persistence.Ef.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Infrastructure.Persistence.Ef.Migrator.dll"]