# Bamboo Card Task

## Overview
This project is a .NET 10 web API application that provides exchange rate services. It includes endpoints for retrieving the latest exchange rates, converting currencies, and more.

## Features
- Retrieve the latest exchange rates for a specific base currency.
- Convert amounts between different currencies.
- Exclude specific currencies from conversion.
- Fetch historical exchange rates with pagination.

## Project Structure
- **BamboCardTask.API**: Contains the main API application.
- **BambooCardTask.Test**: Contains unit tests for the application.

## Prerequisites
- .NET 10 SDK
- Visual Studio or any code editor of your choice.

## Setup
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

## Running the Application
1. Navigate to the API project directory:
   ```bash
   cd BamboCardTask.API
   ```
2. Run the application:
   ```bash
   dotnet run
   ```
3. The API will be available at `http://localhost:5117` by default.

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

## Configuration
- Update `appsettings.json` or `appsettings.Development.json` to configure the `CurrencyExchange` settings:
  ```json
  "CurrencyExchange": {
    "BaseCurrency": "EUR",
    "ExchangeRateApiUrl": "https://api.frankfurter.app/latest"
  }
  ```

## License
This project is licensed under the MIT License.
