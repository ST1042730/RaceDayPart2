

namespace RaceDay.API.Models;

public class EventRoute
{
    public int RouteId { get; set; }

    public int EventId { get; set; }
    public Event? Event { get; set; }

    public string RouteName { get; set; } = string.Empty;

    public decimal DistanceKm { get; set; }

    public string RouteDescription { get; set; } = string.Empty;

    public string MapUrl { get; set; } = string.Empty;
}
