
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using System.Security.Claims;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/enrolments")]
[Authorize]
public class EnrolmentsController(RaceDayDbContext db) : ControllerBase
{
    private int Id
    {
        get
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID missing from token.");

            return int.Parse(userId);
        }
    }

    // PARTICIPANT: Enrol in an event category
    [HttpPost]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Create([FromBody] EnrolmentRequest req)
    {
        var category = await db.EventCategories
            .FirstOrDefaultAsync(ec => ec.EventCategoryId == req.EventCategoryId);

        if (category == null)
            return NotFound("Event category not found.");

        var enrolledCount = await db.Enrolments
            .CountAsync(e => e.EventCategoryId == req.EventCategoryId);

        if (enrolledCount >= category.MaximumParticipants)
            return Conflict("This event category is full.");

        if (await db.Enrolments.AnyAsync(e =>
            e.ParticipantId == Id &&
            e.EventCategoryId == req.EventCategoryId))
            return Conflict("Already enrolled.");

        var enrolment = new Enrolment
        {
            ParticipantId = Id,
            EventCategoryId = req.EventCategoryId,
            EnrolmentDate = DateTime.UtcNow,
            Status = "Registered"
        };

        db.Enrolments.Add(enrolment);
        await db.SaveChangesAsync();

        return Ok(new
        {
            enrolment.EnrolmentId,
            enrolment.ParticipantId,
            enrolment.EventCategoryId,
            enrolment.EnrolmentDate,
            enrolment.Status
        });
    }

    // PARTICIPANT: View own enrolments
    [HttpGet("mine")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Mine()
    {
        var enrolments = await db.Enrolments
            .AsNoTracking()
            .Where(e => e.ParticipantId == Id)
            .Select(e => new
            {
                e.EnrolmentId,
                e.EventCategoryId,
                e.EnrolmentDate,
                e.Status,
                EventId = e.EventCategory!.EventId,
                EventName = e.EventCategory.Event!.EventName,
                EventDate = e.EventCategory.Event.EventDate,
                Location = e.EventCategory.Event.Location,
                CategoryName = e.EventCategory.Category!.CategoryName,
                EntryFee = e.EventCategory.EntryFee
            })
            .ToListAsync();

        return Ok(enrolments);
    }

    // ORGANISER: View participants registered for an event
    [HttpGet("event/{eventId}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> EventEnrolments(int eventId)
    {
        var organiserId = Id;

        var race = await db.Events
            .FirstOrDefaultAsync(e => e.EventId == eventId);

        if (race == null)
            return NotFound("Event not found.");

        if (race.OrganiserId != organiserId)
        {
            return StatusCode(403, new
            {
                message = "This event belongs to a different organiser.",
                eventOrganiserId = race.OrganiserId,
                loggedInOrganiserId = organiserId
            });
        }

        var enrolments = await db.Enrolments
            .AsNoTracking()
            .Include(e => e.Participant)
            .Include(e => e.EventCategory)
            .Where(e => e.EventCategory!.EventId == eventId)
            .Select(e => new
            {
                e.EnrolmentId,
                ParticipantId = e.ParticipantId,
                FirstName = e.Participant!.FirstName,
                LastName = e.Participant.LastName,
                e.EventCategoryId,
                e.EnrolmentDate,
                e.Status
            })
            .ToListAsync();

        return Ok(enrolments);
    }
}

public class EnrolmentRequest
{
    public int EventCategoryId { get; set; }
}
