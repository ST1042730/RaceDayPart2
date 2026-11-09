
namespace RaceDay.MVC.Models;

public class ParticipantResultViewModel
{
    public int ResultId { get; set; }

    public int EnrolmentId { get; set; }

    public TimeSpan FinishTime { get; set; }

    public int Position { get; set; }

    public string ResultStatus { get; set; } = "";

    public DateTime RecordedAt { get; set; }

    public string EventName { get; set; } = "";
}
