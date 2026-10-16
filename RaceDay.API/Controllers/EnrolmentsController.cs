using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using System.Security.Claims;
namespace RaceDay.API.Controllers;
[ApiController,Route("api/enrolments"),Authorize]
public class EnrolmentsController(RaceDayDbContext db):ControllerBase
{
    private int Id=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpPost,Authorize(Roles="Participant")]
    public async Task<IActionResult> Enrol(EnrolRequest req){var ec=await db.EventCategories.Include(x=>x.Event).FirstOrDefaultAsync(x=>x.EventCategoryId==req.EventCategoryId);if(ec is null)return NotFound();if(ec.Event!.EventDate<=DateTime.UtcNow)return BadRequest("Event has passed.");if(await db.Enrolments.AnyAsync(x=>x.ParticipantId==Id&&x.EventCategoryId==req.EventCategoryId))return Conflict("Already enrolled.");if(await db.Enrolments.CountAsync(x=>x.EventCategoryId==req.EventCategoryId)>=ec.MaximumParticipants)return Conflict("Category full.");var e=new Enrolment{ParticipantId=Id,EventCategoryId=req.EventCategoryId};db.Enrolments.Add(e);try{await db.SaveChangesAsync();}catch(DbUpdateException){return Conflict("Could not enrol; check for duplicate or capacity.");}return Ok(new{e.EnrolmentId,e.ParticipantId,e.EventCategoryId});}
    [HttpGet("mine"),Authorize(Roles="Participant")]
    public async Task<IActionResult> Mine()=>Ok(await db.Enrolments.AsNoTracking().Where(e=>e.ParticipantId==Id).Select(e=>new{e.EnrolmentId,e.EventCategoryId,e.EnrolmentDate,e.Status,e.EventCategory!.EventId,e.EventCategory.Event!.EventName}).ToListAsync());
    [HttpGet("event/{eventId:int}"),Authorize(Roles="Organiser")]
    public async Task<IActionResult> Event(int eventId){var ev=await db.Events.FindAsync(eventId);if(ev is null)return NotFound();if(ev.OrganiserId!=Id)return Forbid();return Ok(await db.Enrolments.AsNoTracking().Where(x=>x.EventCategory!.EventId==eventId).Select(x=>new{x.EnrolmentId,x.ParticipantId,x.Participant!.FirstName,x.Participant.LastName,x.EventCategoryId,x.EnrolmentDate,x.Status}).ToListAsync());}
}
