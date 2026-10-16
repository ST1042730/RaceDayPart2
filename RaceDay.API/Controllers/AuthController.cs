using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
namespace RaceDay.API.Controllers;
[ApiController,Route("api/auth")]
public class AuthController(RaceDayDbContext db, IConfiguration config) : ControllerBase
{
    private readonly PasswordHasher<User> hasher = new();
    [HttpPost("register"),AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.FirstName)||string.IsNullOrWhiteSpace(req.LastName)||string.IsNullOrWhiteSpace(req.Email)||string.IsNullOrWhiteSpace(req.Password)) return BadRequest("All fields required.");
        var email=req.Email.Trim().ToLowerInvariant();
        if(await db.Users.AnyAsync(x=>x.Email==email)) return Conflict("Email already registered.");
        var user=new User{FirstName=req.FirstName.Trim(),LastName=req.LastName.Trim(),Email=email,Role="Participant"};
        user.PasswordHash=hasher.HashPassword(user,req.Password);
        db.Users.Add(user);
        try {await db.SaveChangesAsync();} catch(DbUpdateException){return Conflict("Email already registered.");}
        return Created($"/api/profile",new {user.UserId,user.FirstName,user.LastName,user.Email,user.Role});
    }
    [HttpPost("login"),AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var user=await db.Users.FirstOrDefaultAsync(x=>x.Email==req.Email.Trim().ToLower());
        if(user is null || hasher.VerifyHashedPassword(user,user.PasswordHash,req.Password)==PasswordVerificationResult.Failed) return Unauthorized("Invalid credentials.");
        var key=config["Jwt:Key"];
        if(string.IsNullOrWhiteSpace(key)||Encoding.UTF8.GetByteCount(key)<32) return StatusCode(500,"JWT signing key is not configured.");
        var claims=new[]{new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),new Claim(ClaimTypes.Role,user.Role),new Claim(ClaimTypes.Email,user.Email)};
        var token=new JwtSecurityToken(issuer:config["Jwt:Issuer"],audience:config["Jwt:Audience"],claims:claims,expires:DateTime.UtcNow.AddHours(2),signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),SecurityAlgorithms.HmacSha256));
        return Ok(new {token=new JwtSecurityTokenHandler().WriteToken(token),role=user.Role,expiresUtc=token.ValidTo});
    }
}
