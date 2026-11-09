using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models;

public class ProfileViewModel
{
    public int UserId { get; set; }

    [Required]
    public string FirstName { get; set; } = "";

    [Required]
    public string LastName { get; set; } = "";

    public string Email { get; set; } = "";

    public string Role { get; set; } = "";
}