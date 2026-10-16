
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
}
