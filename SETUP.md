
# RaceDay Setup Guide

## Requirements
- .NET 10 SDK
- SQL Server LocalDB
- Visual Studio Code
- Git

## Installation

1. Clone the GitHub repository.
2. Open the RaceDayPart2 folder in VS Code.
3. Restore the project dependencies:

    dotnet restore

4. Configure the database connection string in RaceDay.API/appsettings.json.

5. Initialise local user-secrets:

    dotnet user-secrets init --project RaceDay.API

6. Configure a secure JWT signing key using .NET user-secrets. Never commit the signing key to GitHub.

7. Apply the database migration:

    dotnet ef database update --project RaceDay.API

8. Start the application:

    dotnet run --project RaceDay.API

9. Open Swagger at:

    http://localhost:5074/swagger

## Running Tests

    dotnet test

## GitHub Actions
The GitHub Actions workflow builds and tests the solution automatically when changes are pushed.
