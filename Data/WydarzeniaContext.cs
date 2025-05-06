using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DomKultury.Models;


namespace DomKultury.Data
{
    public class WydarzeniaContext : DbContext
    {
        public WydarzeniaContext(DbContextOptions<WydarzeniaContext> options) : base(options) { }
        public DbSet<Wydarzenie> Wydarzenie { get; set; }

        public DbSet<Kategoria> Kategoria { get; set; }

        public DbSet<Uczestnik> Uczestnik { get; set; }
        public DbSet<Zajecie> Zajecie { get; set; }
        public DbSet<Instruktor> Instruktor { get; set; }
        public DbSet<Konkurs> Konkurs { get; set; }
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
