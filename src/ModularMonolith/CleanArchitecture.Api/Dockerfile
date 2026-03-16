FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN if [ -f "src/ModularMonolith/CleanArchitecture.Api/CleanArchitecture.Api.csproj" ]; then \
      API_PROJECT="src/ModularMonolith/CleanArchitecture.Api/CleanArchitecture.Api.csproj"; \
    elif [ -f "src/CleanArchitecture.Api/CleanArchitecture.Api.csproj" ]; then \
      API_PROJECT="src/CleanArchitecture.Api/CleanArchitecture.Api.csproj"; \
    else \
      echo "Could not find CleanArchitecture.Api.csproj"; \
      exit 1; \
    fi && \
    dotnet restore "$API_PROJECT" && \
    dotnet publish "$API_PROJECT" --configuration Release --output /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CleanArchitecture.Api.dll"]
