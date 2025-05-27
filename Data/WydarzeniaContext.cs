using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DomKultury.Models;
using Microsoft.EntityFrameworkCore;

namespace DomKultury.Data
{
    public class WydarzeniaContext : IdentityDbContext //nie zmieniac tego identity bo utne lapy
    {
        public WydarzeniaContext(DbContextOptions<WydarzeniaContext> options) : base(options) { }

        public DbSet<Wydarzenie> Wydarzenie { get; set; }
        public DbSet<Kategoria> Kategoria { get; set; }
        public DbSet<Uczestnik> Uczestnik { get; set; }
        public DbSet<Zajecie> Zajecie { get; set; }
        public DbSet<Instruktor> Instruktor { get; set; }
        public DbSet<Faq> Faq { get; set; }
        public DbSet<Informacja> Informacja { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // UWAGA: to musi być wyżej niż reszta

            modelBuilder.Entity<Uczestnik>()
                .HasMany(e => e.Zajecia)
                .WithMany(e => e.Uczestnicy)
                .UsingEntity<Dictionary<string, object>>(
                    "UczestnikZajecie",
                    j => j.HasOne<Zajecie>().WithMany().HasForeignKey("ZajecieId"),
                    j => j.HasOne<Uczestnik>().WithMany().HasForeignKey("UczestnikId"));

            modelBuilder.Entity<Zajecie>()
                .HasOne(z => z.Instruktor)
                .WithMany(i => i.Zajecia)
                .HasForeignKey(z => z.InstruktorId);
        }
    }
}
