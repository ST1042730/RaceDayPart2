
# RaceDay Database Design

## Overview
RaceDay uses SQL Server LocalDB with Entity Framework Core to store event management information.

## Database Tables

### 1. Users
Stores registered users, profile information, password hashes and user roles.

### 2. Events
Stores running, walking and cycling events created by organisers.

### 3. Categories
Stores the categories available for event participation.

### 4. EventCategories
Links events to categories and stores category-specific information, such as entry fees and participant limits.

### 5. Enrolments
Records participants registered for event categories.

### 6. Results
Stores race results associated with participant enrolments.

### 7. EventRoutes
Stores route information for events.

### 8. EventWeathers
Stores weather information associated with events.

## Database Relationships
- Users can organise events.
- Events can have multiple categories through EventCategories.
- Participants can enrol in event categories.
- Enrolments can have associated race results.
- Events can have associated route and weather information.

## Entity Framework Core
The project includes an InitialCreate migration to create the database schema.

## Database Setup
The database is configured through the connection string in the API configuration.

Apply the migrations using:

    dotnet ef database update --project RaceDay.API
