FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ProductInventory.sln", "./"]
COPY ["src/ProductInventory.API/ProductInventory.API.csproj", "src/ProductInventory.API/"]
COPY ["src/ProductInventory.Application/ProductInventory.Application.csproj", "src/ProductInventory.Application/"]
COPY ["src/ProductInventory.Domain/ProductInventory.Domain.csproj", "src/ProductInventory.Domain/"]
COPY ["src/ProductInventory.Infrastructure/ProductInventory.Infrastructure.csproj", "src/ProductInventory.Infrastructure/"]

RUN dotnet restore "src/ProductInventory.API/ProductInventory.API.csproj"

COPY . .

WORKDIR "/src/src/ProductInventory.API"

RUN dotnet publish "ProductInventory.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ProductInventory.API.dll"]
