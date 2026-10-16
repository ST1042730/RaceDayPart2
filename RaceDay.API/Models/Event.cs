
namespace RaceDay.API.Models;

public class Event
{
    public int EventId { get; set; }

    public int OrganiserId { get; set; }
    public User? Organiser { get; set; }

    public string EventName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime EventDate { get; set; }

    public string Location { get; set; } = string.Empty;

    public string Status { get; set; } = "Upcoming";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public decimal DistanceKm { get; set; }

    public string EventType { get; set; } = string.Empty;
}

