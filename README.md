# Bamboo Card Task

## Overview
This project is a .NET 10 web API application that provides exchange rate services. It includes endpoints for retrieving the latest exchange rates, converting currencies, and fetching historical exchange rates with pagination.

## Features
- Retrieve the latest exchange rates for a specific base currency.
- Convert amounts between different currencies.
- Exclude specific currencies from conversion.
- Fetch historical exchange rates for a given period with pagination.
- Retry policies with exponential backoff for handling intermittent API failures.
- Response caching for improved performance.

## New Features

### 1. Serilog for Structured Logging
- Integrated Serilog for structured logging.
- Logs are enriched with contextual information and written to the console.
- Configuration for Serilog is read from `appsettings.json`.

### 2. Support for Multiple Exchange Rate Providers
- Added a `Provider` section in `appsettings.json` to specify the active exchange rate provider.
- Dynamically loads provider-specific configurations (e.g., `BaseCurrency`, `ExchangeRateApiUrl`) based on the `Provider:Name` value.
- Supports seamless integration with multiple exchange rate servers.

### 3. Enhanced HttpClient Configuration
- `HttpClient` is dynamically configured using the provider-specific `ExchangeRateApiUrl`.
- Logs the provider name, configuration section, and base address during setup for better observability.

### Example `appsettings.json`
```json
{
  "Provider": {
    "Name": "FrankFurter"
  },
  "FrankFurter": {
    "BaseCurrency": "EUR",
    "ExchangeRateApiUrl": "https://api.frankfurter.dev/v1/",
    "ExcludedCurrencies": [
      "TRY",
      "PLN",
      "THB",
      "MXN"
    ]
  }
}
```


## Project Structure & Key Concepts
- **BamboCardTask.API**: Main API application. Contains:
  - **Routes**: All endpoints are defined in a single place using minimal APIs and grouped for clarity. Each route delegates to a service and uses a shared helper for error handling and logging.
  - **Services**: Business logic is encapsulated in services (e.g., `ExchangeRateService`). Services use a shared private method to handle HTTP and deserialization errors, ensuring DRY code and consistent error handling.
  - **Configuration**: Provider-specific and JWT configuration is loaded from `appsettings.json`.
  - **Middleware**: Custom middleware for logging, correlation IDs, and cache control.
- **BambooCardTask.Test**: Unit and integration tests. Includes:
  - Test authentication handler for simulating authorized/unauthorized requests.
  - Tests for all service and route scenarios, including error and edge cases.
- **BambooCardTask.JwtGenerator**: Generates JWT tokens for local testing and automation.

## Code Organization & Extensibility

### 1. Minimal API & Route Grouping
- All endpoints are defined in `Routes/ExchangeRateRoutes.cs` using minimal API syntax.
- Endpoints are grouped (e.g., `/api/exchange-rates`) and tagged for discoverability.
- Each endpoint delegates to a service and uses a shared helper for error handling and logging, so you only need to focus on business logic.

### 2. Service Layer
- All business logic and external API calls are in the `Services` folder.
- The `ExchangeRateService` uses a single private method (`HandleServiceCall<T>`) to handle HTTP and deserialization errors, so you never need to repeat try/catch blocks.
- To add a new external API or business rule, add a method to the service and call it from a new route.

### 3. Error Handling
- Error handling is DRY: all HTTP and deserialization errors are caught in one place in the service layer.
- Routes use a shared helper to return the correct HTTP status and log errors.

### 4. Logging
- Serilog is used for structured logging throughout the app and tests.
- All service and route actions are logged with context for easy debugging.

### 5. Testing
- Tests use a custom authentication handler to simulate all auth scenarios.
- Tests cover success, error, and edge cases for both services and routes.

### 6. Extending the Codebase
- **To add a new endpoint:**
  1. Add a method to the relevant service (e.g., `ExchangeRateService`).
  2. Add a new route in `ExchangeRateRoutes.cs` that calls your service method and uses the shared error handler.
- **To add a new provider or configuration:**
  1. Add a new section to `appsettings.json`.
  2. Update the provider config and HttpClient setup if needed.
- **To add new tests:**
  1. Add or update test files in `BambooCardTask.Test`.
  2. Use the test authentication handler to simulate different roles and scenarios.


## Prerequisites
- .NET 10 SDK
- Visual Studio or any code editor of your choice.

## Setup and Running the Application

1. Clone the repository:
   ```bash
   git clone https://github.com/chaluvadis/bamboo-card-task.git
   ```
2. Navigate to the project directory:
   ```bash
   cd bamboo-card-task
   ```
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Build the solution:
   ```bash
   dotnet build
   ```
5. Navigate to the API project directory:
   ```bash
   cd BamboCardTask.API
   ```
6. Run the application:
   ```bash
   dotnet run --project BambooCardTask.Api.csproj
   ```

7. The API will be available at `http://localhost:5117` by default.


## Running with Docker (.NET 10 Preview 3) for Dev, Test, and PROD Environments

You can build and run the API in a container using the provided Dockerfile (uses .NET 10 preview 3 images). The solution is ready for deployment in **Development**, **Test**, and **Production** environments using environment variables and configuration files.

### 1. Build the Docker image
From the root of the repository:
```bash
docker build -t bamboo-card-task .
```

### 2. Run the container for a specific environment
You can specify the environment by setting the `ASPNETCORE_ENVIRONMENT` variable. The API will use the corresponding `appsettings.{Environment}.json` file (e.g., `appsettings.Development.json`, `appsettings.Test.json`, `appsettings.Production.json`).

**Development:**
```bash
docker run -p 5117:5117 -e ASPNETCORE_ENVIRONMENT=Development bamboo-card-task
```

**Test:**
```bash
docker run -p 5117:5117 -e ASPNETCORE_ENVIRONMENT=Test bamboo-card-task
```

**Production:**
```bash
docker run -p 5117:5117 -e ASPNETCORE_ENVIRONMENT=Production bamboo-card-task
```

The API will be available at [http://localhost:5117](http://localhost:5117).

#### How it works
- The Dockerfile uses the official .NET 10 preview 3 SDK image to build and publish the app, then runs it with the ASP.NET runtime image.
- The published output is copied to the runtime image for efficient, production-like execution.
- The default port exposed is 5117 (see `EXPOSE 5117` and `ENTRYPOINT` in the Dockerfile).
- The environment is set via the `ASPNETCORE_ENVIRONMENT` variable, which controls which configuration file is loaded.

#### Customizing
- To use a different port, change the `EXPOSE` and `docker run -p` values.
- To set additional environment variables (e.g., connection strings, secrets), use the `-e` flag with `docker run` or add `ENV` lines in the Dockerfile.
- Add or update `appsettings.Development.json`, `appsettings.Test.json`, and `appsettings.Production.json` as needed for each environment.

> **Note:** You must have Docker installed. The .NET 10 preview images are used for both build and runtime stages.

## Testing
1. Navigate to the test project directory:
   ```bash
   cd BambooCardTask.Test
   ```
2. Run the tests:
   ```bash
   dotnet test
   ```

## Testing JWT Authentication

### 1. Generate a JWT Token
- Use a tool like [jwt.io](https://jwt.io/) or write a script to generate a JWT token.
- Ensure the token is signed with the same secret key (`Jwt:Key`) and includes the required claims (e.g., `role`).

Example payload for a user:
```json
{
  "sub": "user@bamboocard.ae",
  "role": "User",
  "iss": "bamboocard.ae",
  "aud": "bamboocard.ae",
  "exp": 1715731200
}
```

Example payload for an admin:
```json
{
  "sub": "admin@bamboocard.ae",
  "role": "Admin",
  "iss": "bamboocard.ae",
  "aud": "bamboocard.ae",
  "exp": 1715731200
}
```

### 2. Add the Token to HTTP Requests
- Include the token in the `Authorization` header of your HTTP requests.

Example:
```http
GET http://localhost:5117/api/exchange-rates/latest
Authorization: Bearer <your-jwt-token>
```

Update your `.http` file to include the `Authorization` header:
```http
@jwt_token = <your-jwt-token>

GET {{bamboo_card_task_HostAddress}}/api/exchange-rates/latest
Authorization: Bearer {{jwt_token}}
Accept: application/json
```

### 3. Test Role-Based Access Control
- Use a token with the `User` role to access user-specific endpoints.
- Use a token with the `Admin` role to access admin-specific endpoints.
- Verify that unauthorized access is denied with a `403 Forbidden` response.

### 4. Automate Testing
- Write integration tests using a library like `xUnit` or `NUnit`.
- Mock the JWT authentication and test the behavior of your endpoints.

## Endpoints
### 1. Retrieve Latest Exchange Rates
- **GET** `/api/exchange-rates/latest?baseCurrency=EUR`

### 2. Currency Conversion
- **POST** `/api/exchange-rates/convert`
  ```json
  {
    "fromCurrency": "EUR",
    "targetCurrencies": ["USD", "AUD"]
  }
  ```

### 3. Historical Exchange Rates with Pagination
- **POST** `/api/exchange-rates/historical`
  ```json
  {
    "startDate": "2025-05-01",
    "endDate": "2025-05-31",
    "fromCurrency": "EUR"
  }
  ```

## Configuration
- Update `appsettings.json` or `appsettings.Development.json` to configure the `CurrencyExchange` settings:
  ```json
  "CurrencyExchange": {
    "BaseCurrency": "EUR",
    "ExchangeRateApiUrl": "https://api.frankfurter.dev/v1/",
    "ExcludedCurrencies": ["TRY", "PLN", "THB", "MXN"]
  }
  ```

## Running the Aspire Project

1. Navigate to the AppHost project directory:
   ```bash
   cd BambooCardTask.AppHost
   ```
2. Run the application:
   ```bash
   dotnet run --project BambooCardTask.AppHost.csproj
   ```
3. The Aspire project will be available at the configured URLs in `appsettings.json` or `appsettings.Development.json`.

### 3. JWT Token Generator
- Introduced the `JwtTokenGenerator` project to facilitate the generation of JWT tokens.
- This project is essential for creating tokens with claims such as `role` and `email`, which are used for authentication and role-based access control (RBAC) in the API.
- Ensures secure and standardized token generation using the secret key defined in the API's configuration.

#### Steps to Run the Project
1. Navigate to the `JwtGenerator` project directory:
   ```bash
   cd BambooCardTask.JwtGenerator
   ```
2. Run the project:
   ```bash
   dotnet run --project BambooCardTask.JwtGenerator.csproj
   ```

#### Sample Output
```
JWT Token for User:
eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJVc2VyIiwianRpIjoiYjU4MDcyMmUtYWIxZi00YmI2LWI4ZTctN2U0NGNmMDdlOTcxIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoidXNlckBiYW1ib29jYXJkLmFlIiwiZXhwIjoxNzQ3MjI0NDQwLCJpc3MiOiJiYW1ib29jYXJkLmFlIiwiYXVkIjoiYmFtYm9vY2FyZC5hZSJ9.IPkZGgwFkFw204G58Gc_EGpIIYlv9DWgqx9GhisvxMi2ffwibcIi1HKJ_10f7nbMnCogwP-ExYIjDw-VqFLxqQ

JWT Token for Admin:
eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImp0aSI6IjliYzI0NWJhLWVkYzgtNDM1NC05N2UzLTEzYTc1OTA5ZDljMSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImFkbWluQGJhbWJvb2NhcmQuYWUiLCJleHAiOjE3NDcyMjQ0NDAsImlzcyI6ImJhbWJvb2NhcmQuYWUiLCJhdWQiOiJiYW1ib29jYXJkLmFlIn0.gFMKKCzTBqJc8Rlc5QGtkZInXdYRbS0E_o7si4Kw0DGG_pROdL8n5LzJ6cYeXHCiEjNf90avpvMgTp5srNV6bg
```

## License
This project is licensed under the MIT License.
