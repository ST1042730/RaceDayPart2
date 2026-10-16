
# RaceDay API Security

## Authentication
RaceDay uses JSON Web Tokens (JWT) to authenticate users.

Users register with an email address and password. Passwords are stored as hashes rather than plain text.

After successful login, the API issues a JWT that can be used to access protected endpoints.

## Role-Based Authorisation
RaceDay supports two roles:
- Organiser
- Participant

Organisers can manage events, categories and race results.

Participants can enrol in event categories and view their own enrolments and results.

## Protected Endpoints
Protected API endpoints require a valid JWT.

Role-based restrictions prevent participants from accessing organiser-only operations.

## Configuration Security
The JWT signing key is stored using .NET user-secrets during local development.

Sensitive signing keys and passwords should not be committed to GitHub.

## Security Testing
Security checks include:
- Successful login with valid credentials
- Rejection of unauthorised access
- Rejection of participant access to organiser-only endpoints
- JWT authentication through Swagger
