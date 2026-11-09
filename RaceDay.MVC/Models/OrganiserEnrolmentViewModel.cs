namespace RaceDay.MVC.Models;

public class OrganiserEnrolmentViewModel
{
    public int EnrolmentId { get; set; }
    public int ParticipantId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public int EventCategoryId { get; set; }
    public DateTime EnrolmentDate { get; set; }
    public string Status { get; set; } = "";

    // Race result details
    public int? ResultId { get; set; }
    public TimeSpan? FinishTime { get; set; }
    public int? Position { get; set; }
    public string? ResultStatus { get; set; }
    public DateTime? RecordedAt { get; set; }
}