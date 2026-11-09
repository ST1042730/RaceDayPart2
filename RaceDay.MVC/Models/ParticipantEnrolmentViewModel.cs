
namespace RaceDay.MVC.Models;

public class ParticipantEnrolmentViewModel
{
    public int EnrolmentId { get; set; }

    public int EventCategoryId { get; set; }

    public DateTime EnrolmentDate { get; set; }

    public string Status { get; set; } = "";

    public int EventId { get; set; }

    public string EventName { get; set; } = "";
}
