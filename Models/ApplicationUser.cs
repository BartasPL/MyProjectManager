using Microsoft.AspNetCore.Identity;

namespace MyProjectManager.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Projekt> MojeProjekty { get; set; } = new List<Projekt>();
    public ICollection<Zadanie> MojeZadania { get; set; } = new List<Zadanie>();
}