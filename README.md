# RaceDay Part 2 – ASP.NET Core Web API

## Project Overview

RaceDay is an event management API designed for running, walking and cycling events in South Africa.

## Technologies Used

* ASP.NET Core Web API (.NET 10)
* C#
* SQL Server LocalDB
* Entity Framework Core
* JWT Authentication
* Swagger
* xUnit
* GitHub Actions

## Features

* User registration and login
* Password hashing
* JWT authentication
* Organiser and Participant roles
* User profile management
* Event management
* Event categories
* Participant enrolment
* Event results

## Database

The application uses eight tables:

* Users
* Events
* Categories
* EventCategories
* Enrolments
* Results
* EventRoutes
* EventWeathers

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



\## MVC Application



The RaceDay MVC application provides a web interface for participants and organisers.



\### Running the MVC Application



1\. Start the RaceDay API first.

2\. Open the RaceDay MVC project.

3\. Run the following command:



```powershell

dotnet run --project RaceDay.MVC

```



4\. Open `http://localhost:5268` in your browser.



\### Running MVC with Docker



Make sure Docker Desktop is running and the RaceDay API is running on port 5074.



Build the Docker image from the solution root:



```powershell

docker build -t raceday-mvc -f RaceDay.MVC/Dockerfile RaceDay.MVC

```



Run the container:



```powershell

docker run --rm -p 8080:8080 -e ApiSettings\_\_BaseUrl=http://host.docker.internal:5074/ raceday-mvc

```



Open `http://localhost:8080` in your browser.



The Docker container runs the MVC application on port 8080 and connects to the API running on the host machine.

