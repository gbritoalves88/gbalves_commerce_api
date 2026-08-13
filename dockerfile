# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["GBAlves.Commerce.Api/GBAlves.Commerce.Api.csproj", "GBAlves.Commerce.Api/"]
COPY ["GBAlves.Commerce.Application/GBAlves.Commerce.Application.csproj", "GBAlves.Commerce.Application/"]
COPY ["GBAlves.Commerce.Domain/GBAlves.Commerce.Domain.csproj", "GBAlves.Commerce.Domain/"]
COPY ["GBAlves.Commerce.Infrastructure/GBAlves.Commerce.Infrastructure.csproj", "GBAlves.Commerce.Infrastructure/"]
RUN dotnet restore "GBAlves.Commerce.Api/GBAlves.Commerce.Api.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/GBAlves.Commerce.Api"
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENTRYPOINT ["dotnet", "GBAlves.Commerce.Api.dll"]