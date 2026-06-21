using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyProjectManager.Models;

namespace MyProjectManager.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Projekt> Projekty { get; set; }
    public DbSet<Zadanie> Zadania { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Projekt>()
            .HasOne(p => p.Owner)
            .WithMany(u => u.MojeProjekty)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.Entity<Zadanie>()
            .HasOne(z => z.Assignee)
            .WithMany(u => u.MojeZadania)
            .HasForeignKey(z => z.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}