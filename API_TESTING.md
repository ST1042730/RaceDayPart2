
# RaceDay API Testing

## Testing Tools
- Swagger UI
- xUnit
- GitHub Actions

## Authentication
- Register a Participant account
- Register an Organiser account
- Log in using valid credentials
- Verify JWT authentication
- Verify role-based access restrictions

## Event Management
- Create an event as an Organiser
- Retrieve the list of events
- Verify that Participants cannot create events

## Categories
- Create an event category
- Assign a category to an event

## Enrolments
- Enrol a Participant in an event category
- View the Participant's enrolments
- Verify duplicate enrolments are rejected

## Results
- Record a Participant's result as an Organiser
- Retrieve results as a Participant

## Unit Tests
Run the automated tests using:

    dotnet test

The project currently includes four passing model tests.

## Continuous Integration
GitHub Actions automatically restores dependencies, builds the solution and runs tests when changes are pushed.
