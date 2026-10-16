
using Xunit;
using RaceDay.API.Models;

namespace RaceDay.Tests;

public class RaceDayModelTests
{
    [Fact]
    public void NewUser_ShouldDefaultToParticipant()
    {
        var user = new User();

        Assert.Equal("Participant", user.Role);
    }

    [Fact]
    public void User_ShouldStoreProfileInformation()
    {
        var user = new User
        {
            FirstName = "Test",
            LastName = "Runner",
            Email = "runner@example.com"
        };

        Assert.Equal("Test", user.FirstName);
        Assert.Equal("Runner", user.LastName);
        Assert.Equal("runner@example.com", user.Email);
    }

    [Fact]
    public void User_ShouldAllowOrganiserRole()
    {
        var user = new User
        {
            Role = "Organiser"
        };

        Assert.Equal("Organiser", user.Role);
    }

    [Fact]
    public void NewUser_ShouldHaveEmptyPasswordHash()
    {
        var user = new User();

        Assert.Equal(string.Empty, user.PasswordHash);
    }

    [Fact]
    public void User_Email_CanBeUpdated()
    {
        var user = new User
        {
            Email = "old@example.com"
        };

        user.Email = "new@example.com";

        Assert.Equal("new@example.com", user.Email);
    }

    [Fact]
    public void User_LastName_CanBeUpdated()
    {
        var user = new User
        {
            LastName = "Smith"
        };

        user.LastName = "Johnson";

        Assert.Equal("Johnson", user.LastName);
    }

    [Fact]
    public void Event_ShouldStoreEventName()
    {
        var raceEvent = new Event
        {
            EventName = "Johannesburg 5K"
        };

        Assert.Equal("Johannesburg 5K", raceEvent.EventName);
    }

    [Fact]
    public void NewEvent_ShouldDefaultToUpcoming()
    {
        var raceEvent = new Event();

        Assert.Equal("Upcoming", raceEvent.Status);
    }

    [Fact]
    public void Event_ShouldStoreDistanceInKilometres()
    {
        var raceEvent = new Event
        {
            EventName = "Johannesburg 10K",
            DistanceKm = 10.5m
        };

        Assert.Equal(10.5m, raceEvent.DistanceKm);
    }

    [Fact]
    public void NewEvent_ShouldHaveEmptyName()
    {
        var raceEvent = new Event();

        Assert.Equal(string.Empty, raceEvent.EventName);
    }
}
