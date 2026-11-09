
using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models;

public class RecordResultViewModel
{
    [Required]
    public int EnrolmentId { get; set; }

    [Required]
    public TimeSpan FinishTime { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Position { get; set; }

    public string ResultStatus { get; set; } = "Completed";
}
