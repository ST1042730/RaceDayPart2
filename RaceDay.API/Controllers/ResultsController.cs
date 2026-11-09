
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using System.Security.Claims;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/results")]
[Authorize]
public class ResultsController(RaceDayDbContext db) : ControllerBase
{
    private int Id => int.Parse(
        User.FindFirstValue(ClaimTypes.NameIdentifier)!
    );

    // ORGANISER: Record a race result
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Record(ResultRequest req)
    {
        var enrolment = await db.Enrolments
            .Include(e => e.EventCategory)
            .FirstOrDefaultAsync(e => e.EnrolmentId == req.EnrolmentId);

        if (enrolment == null)
            return NotFound("Enrolment not found.");

        if (enrolment.EventCategory == null)
            return BadRequest("Enrolment has no event category.");

        var race = await db.Events.FindAsync(
            enrolment.EventCategory.EventId
        );

        if (race == null)
            return NotFound("Event not found.");

        if (race.OrganiserId != Id)
            return Forbid();

        if (req.FinishTime <= TimeSpan.Zero ||
            req.FinishTime >= TimeSpan.FromDays(1) ||
            req.Position <= 0)
        {
            return BadRequest("Invalid finish time or position.");
        }

        if (await db.Results.AnyAsync(
            r => r.EnrolmentId == req.EnrolmentId))
        {
            return Conflict("Result already exists.");
        }

        var result = new Result
        {
            EnrolmentId = req.EnrolmentId,
            FinishTime = req.FinishTime,
            Position = req.Position,
            ResultStatus = req.ResultStatus
        };

        db.Results.Add(result);
        await db.SaveChangesAsync();

        return Ok(new
        {
            result.ResultId,
            result.EnrolmentId,
            result.FinishTime,
            result.Position,
            result.ResultStatus,
            result.RecordedAt
        });
    }

    // ORGANISER: View all enrolments and results for an event
    [HttpGet("event/{eventId}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> EventResults(int eventId)
    {
        var race = await db.Events
            .FirstOrDefaultAsync(e => e.EventId == eventId);

        if (race == null)
            return NotFound("Event not found.");

        if (race.OrganiserId != Id)
            return Forbid();

        var enrolments = await db.Enrolments
            .AsNoTracking()
            .Where(e => e.EventCategory!.EventId == eventId)
            .Select(e => new
            {
                e.EnrolmentId,
                ParticipantId = e.ParticipantId,
                FirstName = e.Participant!.FirstName,
                LastName = e.Participant.LastName,
                e.EventCategoryId,
                e.EnrolmentDate,
                e.Status,

                ResultId = db.Results
                    .Where(r => r.EnrolmentId == e.EnrolmentId)
                    .Select(r => (int?)r.ResultId)
                    .FirstOrDefault(),

                FinishTime = db.Results
                    .Where(r => r.EnrolmentId == e.EnrolmentId)
                    .Select(r => (TimeSpan?)r.FinishTime)
                    .FirstOrDefault(),

                Position = db.Results
                    .Where(r => r.EnrolmentId == e.EnrolmentId)
                    .Select(r => (int?)r.Position)
                    .FirstOrDefault(),

                ResultStatus = db.Results
                    .Where(r => r.EnrolmentId == e.EnrolmentId)
                    .Select(r => r.ResultStatus)
                    .FirstOrDefault(),

                RecordedAt = db.Results
                    .Where(r => r.EnrolmentId == e.EnrolmentId)
                    .Select(r => (DateTime?)r.RecordedAt)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(enrolments);
    }

    // PARTICIPANT: View own race results
    [HttpGet("mine")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Mine()
    {
        var results = await db.Results
            .AsNoTracking()
            .Where(r => r.Enrolment!.ParticipantId == Id)
            .Select(r => new
            {
                r.ResultId,
                r.EnrolmentId,
                r.FinishTime,
                r.Position,
                r.ResultStatus,
                r.RecordedAt,
                EventName = r.Enrolment!.EventCategory!
                    .Event!.EventName
            })
            .ToListAsync();

        return Ok(results);
    }
}
