
namespace RaceDay.API.Models;

public class Enrolment
{
    public int EnrolmentId { get; set; }

    public int ParticipantId { get; set; }
    public User? Participant { get; set; }

    public int EventCategoryId { get; set; }
    public EventCategory? EventCategory { get; set; }

    public DateTime EnrolmentDate { get; set; }
        = DateTime.UtcNow;

    public string Status { get; set; }
        = "Registered";
}
