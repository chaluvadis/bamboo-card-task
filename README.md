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

## Project Structure
- **BamboCardTask.API**: Contains the main API application, including controllers, services, and configuration.
- **BambooCardTask.Test**: Contains unit tests for the application.
- **BambooCardTask.ServiceDefaults**: Provides shared service configurations, including resilience and observability.
- **BambooCardTask.AppHost**: Hosts the application and manages startup configurations.

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

## Testing
1. Navigate to the test project directory:
   ```bash
   cd BambooCardTask.Test
   ```
2. Run the tests:
   ```bash
   dotnet test
   ```

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

## License
This project is licensed under the MIT License.
