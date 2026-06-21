namespace MyProjectManager.Models;

public class Zadanie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsZrobione { get; set; } 

    public int ProjektId { get; set; }
    public Projekt Projekt { get; set; } = null!;

    public string? AssigneeId { get; set; }
    public ApplicationUser? Assignee { get; set; }
}