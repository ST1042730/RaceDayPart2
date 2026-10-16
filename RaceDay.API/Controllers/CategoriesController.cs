using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using System.Security.Claims;
namespace RaceDay.API.Controllers;
[ApiController,Route("api/categories")]
public class CategoriesController(RaceDayDbContext db):ControllerBase
{
    [HttpGet,AllowAnonymous]public async Task<IActionResult> All()=>Ok(await db.Categories.AsNoTracking().ToListAsync());
    [HttpPost,Authorize(Roles="Organiser")]
    public async Task<IActionResult> Create(CategoryRequest req){if(string.IsNullOrWhiteSpace(req.CategoryName))return BadRequest();var c=new Category{CategoryName=req.CategoryName,Description=req.Description};db.Categories.Add(c);await db.SaveChangesAsync();return Ok(c);}
    [HttpGet("event/{eventId:int}"),AllowAnonymous]public async Task<IActionResult> ForEvent(int eventId)=>Ok(await db.EventCategories.AsNoTracking().Where(x=>x.EventId==eventId).Select(x=>new{x.EventCategoryId,x.EventId,x.CategoryId,x.Category!.CategoryName,x.EntryFee,x.MaximumParticipants}).ToListAsync());
    [HttpPost("event"),Authorize(Roles="Organiser")]
    public async Task<IActionResult> Assign(EventCategoryRequest req){var id=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);var e=await db.Events.FindAsync(req.EventId);if(e is null||!await db.Categories.AnyAsync(c=>c.CategoryId==req.CategoryId))return NotFound();if(e.OrganiserId!=id)return Forbid();if(req.MaximumParticipants<=0||req.EntryFee<0)return BadRequest();if(await db.EventCategories.AnyAsync(x=>x.EventId==req.EventId&&x.CategoryId==req.CategoryId))return Conflict("Category already assigned.");var x=new EventCategory{EventId=req.EventId,CategoryId=req.CategoryId,EntryFee=req.EntryFee,MaximumParticipants=req.MaximumParticipants};db.EventCategories.Add(x);await db.SaveChangesAsync();return Ok(new{x.EventCategoryId,x.EventId,x.CategoryId,x.EntryFee,x.MaximumParticipants});}
}
