namespace MyProjectManager.Models;

public class Projekt
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;

    public string OwnerId { get; set; } = string.Empty;
    public ApplicationUser Owner { get; set; } = null!;

    public ICollection<Zadanie> Tasks { get; set; } = new List<Zadanie>();
}