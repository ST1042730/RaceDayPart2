
using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models;

public class CreateEventViewModel
{
    public int EventId { get; set; }

    [Required]
    public string EventName { get; set; } = "";

    public string Description { get; set; } = "";

    [Required]
    public DateTime EventDate { get; set; }

    [Required]
    public string Location { get; set; } = "";

    [Range(0.1, 1000)]
    public decimal DistanceKm { get; set; }

    [Required]
    public string EventType { get; set; } = "Run";

    public string Status { get; set; } = "Upcoming";
}

