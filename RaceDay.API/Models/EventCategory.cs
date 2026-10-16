
namespace RaceDay.API.Models;

public class EventCategory
{
    public int EventCategoryId { get; set; }

    public int EventId { get; set; }
    public Event? Event { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public decimal EntryFee { get; set; }

    public int MaximumParticipants { get; set; }
}

