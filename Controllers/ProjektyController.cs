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
public class ProjektyController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjektyController(AppDbContext context)
    {
        _context = context;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyProjekty()
    {
        var currentUserId = GetUserId();

        var projekty = await _context.Projekty
            .Include(p => p.Tasks)
            .Where(p => p.OwnerId == currentUserId || p.Tasks.Any(z => z.AssigneeId == currentUserId))
            .ToListAsync();

        return Ok(projekty);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProjekt([FromBody] CreateProjektDto dto)
    {
        var currentUserId = GetUserId();

        var nowyProjekt = new Projekt
        {
            Name = dto.Name,
            Opis = dto.Opis,
            OwnerId = currentUserId
        };

        _context.Projekty.Add(nowyProjekt);
        await _context.SaveChangesAsync();

        return Ok(nowyProjekt);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProjekt(int id, [FromBody] CreateProjektDto dto)
    {
        var currentUserId = GetUserId();
        var projekt = await _context.Projekty.FirstOrDefaultAsync(p => p.Id == id);

        if (projekt == null) return NotFound("Projekt nie istnieje.");
        
        if (projekt.OwnerId != currentUserId) return Forbid(); 

        projekt.Name = dto.Name;
        projekt.Opis = dto.Opis;

        await _context.SaveChangesAsync();
        return Ok(projekt);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjekt(int id)
    {
        var currentUserId = GetUserId();
        var projekt = await _context.Projekty.FirstOrDefaultAsync(p => p.Id == id);

        if (projekt == null) return NotFound("Projekt nie istnieje.");
        
        if (projekt.OwnerId != currentUserId) return Forbid(); 

        _context.Projekty.Remove(projekt);
        await _context.SaveChangesAsync();
        return Ok("Projekt usunięty bezpowrotnie!");
    }
}

public class CreateProjektDto
{
    public string Name { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;
}