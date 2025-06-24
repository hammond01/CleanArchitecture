# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/Presentation/APIs/ProductManager.Api/ProductManager.Api.csproj", "src/Presentation/APIs/ProductManager.Api/"]
COPY ["src/Application/ProductManager.Application/ProductManager.Application.csproj", "src/Application/ProductManager.Application/"]
COPY ["src/Application/ProductManager.Domain/ProductManager.Domain.csproj", "src/Application/ProductManager.Domain/"]
COPY ["src/Infrastructure/ProductManager.Infrastructure/ProductManager.Infrastructure.csproj", "src/Infrastructure/ProductManager.Infrastructure/"]
COPY ["src/Infrastructure/ProductManager.Persistence/ProductManager.Persistence.csproj", "src/Infrastructure/ProductManager.Persistence/"]
COPY ["src/CrossCuttingConcerns/ProductManager.Shared/ProductManager.Shared.csproj", "src/CrossCuttingConcerns/ProductManager.Shared/"]
COPY ["src/CrossCuttingConcerns/ProductManager.Constants/ProductManager.Constants.csproj", "src/CrossCuttingConcerns/ProductManager.Constants/"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

RUN dotnet restore "src/Presentation/APIs/ProductManager.Api/ProductManager.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Presentation/APIs/ProductManager.Api"
RUN dotnet build "ProductManager.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ProductManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

EXPOSE 8080
ENTRYPOINT ["dotnet", "ProductManager.Api.dll"]
