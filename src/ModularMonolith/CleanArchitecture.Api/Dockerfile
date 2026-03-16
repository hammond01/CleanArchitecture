# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["NuGet.Config", "."]
COPY ["src/ModularMonolith/CleanArchitecture.Api/CleanArchitecture.Api.csproj", "src/ModularMonolith/CleanArchitecture.Api/"]
COPY ["src/ModularMonolith/BuildingBlocks/BuildingBlocks.Api/BuildingBlocks.Api.csproj", "src/ModularMonolith/BuildingBlocks/BuildingBlocks.Api/"]
COPY ["src/ModularMonolith/BuildingBlocks/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "src/ModularMonolith/BuildingBlocks/BuildingBlocks.Application/"]
COPY ["src/ModularMonolith/BuildingBlocks/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "src/ModularMonolith/BuildingBlocks/BuildingBlocks.Domain/"]
COPY ["src/ModularMonolith/BuildingBlocks/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj", "src/ModularMonolith/BuildingBlocks/BuildingBlocks.Infrastructure/"]
COPY ["src/ModularMonolith/BuildingBlocks/BuildingBlocks.Shared/BuildingBlocks.Shared.csproj", "src/ModularMonolith/BuildingBlocks/BuildingBlocks.Shared/"]
COPY ["src/ModularMonolith/Modules/Catalog/Catalog.Api/Catalog.Api.csproj", "src/ModularMonolith/Modules/Catalog/Catalog.Api/"]
COPY ["src/ModularMonolith/Modules/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "src/ModularMonolith/Modules/Catalog/Catalog.Infrastructure/"]
COPY ["src/ModularMonolith/Modules/Catalog/Catalog.Application/Catalog.Application.csproj", "src/ModularMonolith/Modules/Catalog/Catalog.Application/"]
COPY ["src/ModularMonolith/Modules/Catalog/Catalog.Domain/Catalog.Domain.csproj", "src/ModularMonolith/Modules/Catalog/Catalog.Domain/"]
COPY ["src/ModularMonolith/Modules/Identity/Identity.Api/Identity.Api.csproj", "src/ModularMonolith/Modules/Identity/Identity.Api/"]
COPY ["src/ModularMonolith/Modules/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "src/ModularMonolith/Modules/Identity/Identity.Infrastructure/"]
COPY ["src/ModularMonolith/Modules/Identity/Identity.Application/Identity.Application.csproj", "src/ModularMonolith/Modules/Identity/Identity.Application/"]
COPY ["src/ModularMonolith/Modules/Identity/Identity.Domain/Identity.Domain.csproj", "src/ModularMonolith/Modules/Identity/Identity.Domain/"]
COPY ["src/ModularMonolith/Modules/Auditing/Auditing.Api/Auditing.Api.csproj", "src/ModularMonolith/Modules/Auditing/Auditing.Api/"]
COPY ["src/ModularMonolith/Modules/Auditing/Auditing.Application/Auditing.Application.csproj", "src/ModularMonolith/Modules/Auditing/Auditing.Application/"]
COPY ["src/ModularMonolith/Modules/Auditing/Auditing.Domain/Auditing.Domain.csproj", "src/ModularMonolith/Modules/Auditing/Auditing.Domain/"]
COPY ["src/ModularMonolith/Modules/Auditing/Auditing.Infrastructure/Auditing.Infrastructure.csproj", "src/ModularMonolith/Modules/Auditing/Auditing.Infrastructure/"]
RUN dotnet restore "./src/ModularMonolith/CleanArchitecture.Api/CleanArchitecture.Api.csproj"
COPY . .
WORKDIR "/src/src/ModularMonolith/CleanArchitecture.Api"
RUN dotnet build "./CleanArchitecture.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CleanArchitecture.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CleanArchitecture.Api.dll"]