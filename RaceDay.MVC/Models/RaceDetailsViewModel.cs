
namespace RaceDay.MVC.Models;

public class RaceDetailsViewModel
{
    public EventViewModel Event { get; set; } = new();

    public List<EventCategoryViewModel> Categories { get; set; } = new();
}
