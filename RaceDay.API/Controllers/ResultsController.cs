using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using System.Security.Claims;
namespace RaceDay.API.Controllers;
[ApiController,Route("api/results"),Authorize]
public class ResultsController(RaceDayDbContext db):ControllerBase
{
    private int Id=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpPost,Authorize(Roles="Organiser")]
    public async Task<IActionResult> Record(ResultRequest req){var e=await db.Enrolments.Include(x=>x.EventCategory).FirstOrDefaultAsync(x=>x.EnrolmentId==req.EnrolmentId);if(e is null)return NotFound();var ev=await db.Events.FindAsync(e.EventCategory!.EventId);if(ev!.OrganiserId!=Id)return Forbid();if(req.FinishTime<=TimeSpan.Zero||req.FinishTime>=TimeSpan.FromDays(1)||req.Position<=0)return BadRequest("Invalid finish time or position.");if(await db.Results.AnyAsync(r=>r.EnrolmentId==req.EnrolmentId))return Conflict("Result already exists.");var r=new Result{EnrolmentId=req.EnrolmentId,FinishTime=req.FinishTime,Position=req.Position,ResultStatus=req.ResultStatus};db.Results.Add(r);await db.SaveChangesAsync();return Ok(new{r.ResultId,r.EnrolmentId,r.FinishTime,r.Position,r.ResultStatus});}
    [HttpGet("mine"),Authorize(Roles="Participant")]
    public async Task<IActionResult> Mine()=>Ok(await db.Results.AsNoTracking().Where(r=>r.Enrolment!.ParticipantId==Id).Select(r=>new{r.ResultId,r.EnrolmentId,r.FinishTime,r.Position,r.ResultStatus,r.RecordedAt,r.Enrolment!.EventCategory!.Event!.EventName}).ToListAsync());
}
