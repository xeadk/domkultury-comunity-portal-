using DomKultury.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DomKultury.Data
{
    public class UczestnicyContext : IdentityDbContext
    {
        public UczestnicyContext(DbContextOptions<UczestnicyContext> options) : base(options) { }

        public DbSet<Uczestnik> Uczestnik { get; set; }
        public DbSet<Zajecie> Zajecie { get; set; }

        // DbSet dla Instruktorów
        public DbSet<Instruktor> Instruktor { get; set; }
        protected override void OnModelCreating(ModelBuilder
        modelBuilder)
        {
            // Relacja Many-to-Many z Uczestnikami i Zajęciami
            modelBuilder.Entity<Uczestnik>()
            .HasMany(e => e.Zajecia)
            .WithMany(e => e.Uczestnicy)
            // mimo, że vs sam stworzy tabelę pośredniczącą, to samemu nadamy jej nazwę
            .UsingEntity<Dictionary<string, object>>(
            "UczestnikZajecie", // nazwa tabeli pośredniczącej
            j => j.HasOne<Zajecie>().WithMany().HasForeignKey("ZajecieId"),
            j => j.HasOne<Uczestnik>().WithMany().HasForeignKey("UczestnikId"));

            // Relacja One-to-Many z Instruktorem
            modelBuilder.Entity<Zajecie>()
                .HasOne(z => z.Instruktor)
                .WithMany(i => i.Zajecia)
                .HasForeignKey(z => z.InstruktorId);
            base.OnModelCreating(modelBuilder); //logowanie
        }
    }
}
