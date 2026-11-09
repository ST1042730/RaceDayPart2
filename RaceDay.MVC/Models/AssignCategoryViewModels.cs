using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models;

public class AssignCategoryViewModel
{
    [Required]
    public int EventId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Range(0, 100000)]
    public decimal EntryFee { get; set; }

    [Range(1, 10000)]
    public int MaximumParticipants { get; set; }
}