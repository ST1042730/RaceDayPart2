
# RaceDay Part 2 – ASP.NET Core Web API

## Project Overview
RaceDay is an event management API designed for running, walking and cycling events in South Africa.

## Technologies Used
- ASP.NET Core Web API (.NET 10)
- C#
- SQL Server LocalDB
- Entity Framework Core
- JWT Authentication
- Swagger
- xUnit
- GitHub Actions

## Features
- User registration and login
- Password hashing
- JWT authentication
- Organiser and Participant roles
- User profile management
- Event management
- Event categories
- Participant enrolment
- Event results

## Database
The application uses eight tables:
- Users
- Events
- Categories
- EventCategories
- Enrolments
- Results
- EventRoutes
- EventWeathers

## Running the Application
1. Install the .NET 10 SDK.
2. Configure SQL Server and the connection string.
3. Configure Jwt:Key using .NET user-secrets.
4. Apply the Entity Framework Core migrations.
5. Run the API using:

   dotnet run --project RaceDay.API

6. Open the Swagger URL displayed when the API starts.

## Testing
Run the unit tests using:

   dotnet test

## CI/CD
GitHub Actions automatically builds and tests the project whenever changes are pushed.

## Author
ST1042730
