

namespace RaceDay.API.Models;

public class EventWeather
{
    public int WeatherId { get; set; }

    public int EventId { get; set; }
    public Event? Event { get; set; }

    public decimal Temperature { get; set; }

    public string WeatherCondition { get; set; } = string.Empty;

    public decimal WindSpeed { get; set; }

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}
