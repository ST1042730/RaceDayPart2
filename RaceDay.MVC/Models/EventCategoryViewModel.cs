
namespace RaceDay.MVC.Models;

public class EventCategoryViewModel
{
    public int EventCategoryId { get; set; }
    public int EventId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public decimal EntryFee { get; set; }
    public int MaximumParticipants { get; set; }
}
