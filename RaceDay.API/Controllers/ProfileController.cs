using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
namespace RaceDay.API.Controllers;
[ApiController,Route("api/profile"),Authorize]
public class ProfileController(RaceDayDbContext db):ControllerBase
{
    private int Id=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpGet]
    public async Task<IActionResult> Get(){var u=await db.Users.FindAsync(Id);return u is null?NotFound():Ok(new{u.UserId,u.FirstName,u.LastName,u.Email,u.Role});}
    [HttpPut]
    public async Task<IActionResult> Update(ProfileUpdateRequest req){var u=await db.Users.FindAsync(Id);if(u is null)return NotFound();u.FirstName=req.FirstName.Trim();u.LastName=req.LastName.Trim();await db.SaveChangesAsync();return Ok(new{u.UserId,u.FirstName,u.LastName,u.Email,u.Role});}
}
