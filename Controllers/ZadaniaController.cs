using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProjectManager.Data;
using MyProjectManager.Models;
using System.Security.Claims;

namespace MyProjectManager.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ZadaniaController : ControllerBase
{
    private readonly AppDbContext _context;

    public ZadaniaController(AppDbContext context)
    {
        _context = context;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetAllZadania()
    {
        var userId = GetUserId();

        var zadania = await _context.Zadania
            .Include(z => z.Projekt)
            .Where(z => z.AssigneeId == userId || z.Projekt.OwnerId == userId)
            .ToListAsync();

        return Ok(zadania);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetZadanieById(int id)
    {
        var userId = GetUserId();
        var zadanie = await _context.Zadania
            .Include(z => z.Projekt)
            .FirstOrDefaultAsync(z => z.Id == id);

        if (zadanie == null) return NotFound("Nie ma takiego zadania.");

        if (zadanie.AssigneeId != userId && zadanie.Projekt.OwnerId != userId)
        {
            return Forbid();
        }

        return Ok(zadanie);
    }

    [HttpPost]
    public async Task<IActionResult> CreateZadanie([FromBody] CreateZadanieDto dto)
    {
        var userId = GetUserId();

        var projekt = await _context.Projekty.FirstOrDefaultAsync(p => p.Id == dto.ProjektId);
        if (projekt == null) return NotFound("Projekt nie istnieje.");
        
        if (projekt.OwnerId != userId)
        {
            return Forbid();
        }

        var noweZadanie = new Zadanie
        {
            Title = dto.Title,
            IsZrobione = false,
            ProjektId = dto.ProjektId,
            AssigneeId = dto.AssigneeId
        };

        _context.Zadania.Add(noweZadanie);
        await _context.SaveChangesAsync();

        return Ok(noweZadanie);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateZadanie(int id, [FromBody] UpdateZadanieDto dto)
    {
        var userId = GetUserId();
        var zadanie = await _context.Zadania
            .Include(z => z.Projekt)
            .FirstOrDefaultAsync(z => z.Id == id);

        if (zadanie == null) return NotFound("Zadanie nie istnieje.");

        bool isOwner = zadanie.Projekt.OwnerId == userId;
        bool isAssignee = zadanie.AssigneeId == userId;

        if (!isOwner && !isAssignee)
        {
            return Forbid();
        }

        if (isOwner)
        {
            zadanie.Title = dto.Title;
            zadanie.IsZrobione = dto.IsZrobione;
            zadanie.AssigneeId = dto.AssigneeId;
        }
        else if (isAssignee)
        {
            zadanie.IsZrobione = dto.IsZrobione;
        }

        await _context.SaveChangesAsync();
        return Ok(zadanie);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteZadanie(int id)
    {
        var userId = GetUserId();
        var zadanie = await _context.Zadania
            .Include(z => z.Projekt)
            .FirstOrDefaultAsync(z => z.Id == id);

        if (zadanie == null) return NotFound("Zadanie nie istnieje.");

        if (zadanie.Projekt.OwnerId != userId)
        {
            return Forbid();
        }

        _context.Zadania.Remove(zadanie);
        await _context.SaveChangesAsync();

        return Ok("Zadanie usunięte pomyślnie!");
    }
}

public class CreateZadanieDto
{
    public string Title { get; set; } = string.Empty;
    public int ProjektId { get; set; }
    public string? AssigneeId { get; set; }
}

public class UpdateZadanieDto
{
    public string Title { get; set; } = string.Empty;
    public bool IsZrobione { get; set; }
    public string? AssigneeId { get; set; }
}