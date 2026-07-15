# Product Inventory Management API

A RESTful backend API for managing products and inventory quantities, developed using .NET 8, ASP.NET Core Web API, Entity Framework Core and SQL Server.

## Features

- Product CRUD operations
- Inventory quantity management
- Pagination, search and sorting
- JWT authentication
- Refresh-token rotation
- User and Admin roles
- Admin-only product deletion
- FluentValidation
- Global exception handling
- Swagger/OpenAPI with JWT support
- Repository and Unit of Work patterns
- xUnit and Moq tests
- Docker and Docker Compose configuration

## Technology Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server
- JWT Bearer Authentication
- FluentValidation
- Swagger
- xUnit
- Moq
- Docker

## Project Structure

ProductInventoryManagement/
  backend/
    src/
      ProductInventory.API/
      ProductInventory.Application/
      ProductInventory.Domain/
      ProductInventory.Infrastructure/
    tests/
      ProductInventory.API.Tests/
      ProductInventory.Application.Tests/
      ProductInventory.Infrastructure.Tests/
    Dockerfile
    ProductInventory.sln
  docker-compose.yml
  .env.example
  .gitignore
  global.json
  README.md

## API Endpoints

### Authentication

- POST /api/auth/register
- POST /api/auth/login
- POST /api/auth/refresh

### Products

- GET /api/products
- GET /api/products/{id}
- POST /api/products
- PUT /api/products/{id}
- PUT /api/products/{id}/quantity
- DELETE /api/products/{id} - Admin only

## Pagination, Search and Sorting

Examples:

GET /api/products?pageNumber=1&pageSize=10
GET /api/products?search=laptop
GET /api/products?sortBy=quantity&sortDirection=desc

## Run Locally

Requirements:

- .NET 8 SDK
- SQL Server LocalDB or SQL Server
- Entity Framework Core CLI tools

From the repository root:

cd backend
dotnet restore
dotnet build

Apply database migrations:

dotnet ef database update --project src/ProductInventory.Infrastructure --startup-project src/ProductInventory.API

Run the API:

dotnet run --project src/ProductInventory.API

Swagger is available at:

http://localhost:5195/swagger

The exact port may differ depending on the local environment.

## Authentication

Register or log in through Swagger. Copy the returned accessToken, click Authorize and paste the token.

Refresh tokens are stored as SHA-256 hashes and rotated after use.

## Admin Account

Set temporary Admin credentials before running the API:

$env:SeedAdmin__Username="admin"
$env:SeedAdmin__Password="YourStrongPassword"

Do not commit actual credentials.

## Tests

Run:

cd backend
dotnet test

Verified result:

- Passed: 2
- Failed: 0

## Docker

Docker configuration is included through backend/Dockerfile and docker-compose.yml.

Create a local .env file using .env.example and run:

docker compose up --build

Docker configuration was not locally executed because Docker Desktop was unavailable.

## Environment Variables

- ConnectionStrings__DefaultConnection
- Jwt__Issuer
- Jwt__Audience
- Jwt__SecretKey
- Jwt__AccessTokenMinutes
- Jwt__RefreshTokenDays
- SeedAdmin__Username
- SeedAdmin__Password

## Database

Each Product has zero or one Item inventory record.

Migrations:

- InitialProductAndItemSchema
- AddAuthenticationTables

## Security

- Password hashing
- Short-lived JWT access tokens
- Refresh-token rotation and revocation
- Role-based authorization
- Request validation
- Safe ProblemDetails error responses
- Environment-based secrets

## Verification

The following were tested locally:

- Product creation, retrieval, update and deletion
- Inventory quantity update
- Pagination, search and sorting
- Registration and login
- JWT authorization
- Refresh-token rotation
- Admin-only deletion
- Database migrations
- Successful build
- Passing unit tests

## Notes

This repository contains the backend solution for the Product Inventory Management technical assessment.

No real passwords, JWT secrets, refresh tokens or personal credentials should be committed.
