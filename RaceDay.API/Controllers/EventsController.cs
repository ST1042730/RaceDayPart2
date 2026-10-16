using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
namespace RaceDay.API.Controllers;
[ApiController,Route("api/events"),Authorize]
public class EventsController(RaceDayDbContext db):ControllerBase
{
    private int Id=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpGet,AllowAnonymous] public async Task<IActionResult> All()=>Ok(await db.Events.AsNoTracking().Select(e=>new{e.EventId,e.EventName,e.Description,e.EventDate,e.Location,e.DistanceKm,e.EventType,e.Status,e.OrganiserId}).ToListAsync());
    [HttpGet("{id:int}"),AllowAnonymous] public async Task<IActionResult> One(int id){var e=await db.Events.AsNoTracking().Where(e=>e.EventId==id).Select(e=>new{e.EventId,e.EventName,e.Description,e.EventDate,e.Location,e.DistanceKm,e.EventType,e.Status,e.OrganiserId}).FirstOrDefaultAsync();return e is null?NotFound():Ok(e);}
    [HttpPost,Authorize(Roles="Organiser")]
    public async Task<IActionResult> Create(EventRequest req){if(!Valid(req))return BadRequest("Check event type, date, distance and required fields.");var e=new Event{OrganiserId=Id,EventName=req.EventName,Description=req.Description,EventDate=req.EventDate,Location=req.Location,DistanceKm=req.DistanceKm,EventType=req.EventType,Status=req.Status};db.Events.Add(e);await db.SaveChangesAsync();return CreatedAtAction(nameof(One),new{id=e.EventId},new{e.EventId,e.EventName});}
    [HttpPut("{id:int}"),Authorize(Roles="Organiser")]
    public async Task<IActionResult> Update(int id,EventRequest req){var e=await db.Events.FindAsync(id);if(e is null)return NotFound();if(e.OrganiserId!=Id)return Forbid();if(!Valid(req))return BadRequest("Invalid event data.");e.EventName=req.EventName;e.Description=req.Description;e.EventDate=req.EventDate;e.Location=req.Location;e.DistanceKm=req.DistanceKm;e.EventType=req.EventType;e.Status=req.Status;await db.SaveChangesAsync();return NoContent();}
    [HttpDelete("{id:int}"),Authorize(Roles="Organiser")]
    public async Task<IActionResult> Delete(int id){var e=await db.Events.FindAsync(id);if(e is null)return NotFound();if(e.OrganiserId!=Id)return Forbid();if(await db.Enrolments.AnyAsync(n=>n.EventCategory!.EventId==id))return Conflict("Cannot delete an event with enrolments.");db.Events.Remove(e);await db.SaveChangesAsync();return NoContent();}
    private static bool Valid(EventRequest r)=>!string.IsNullOrWhiteSpace(r.EventName)&&!string.IsNullOrWhiteSpace(r.Location)&&r.DistanceKm>0&&new[]{"Run","Walk","Cycle"}.Contains(r.EventType,StringComparer.OrdinalIgnoreCase)&&r.EventDate!=default;
}
