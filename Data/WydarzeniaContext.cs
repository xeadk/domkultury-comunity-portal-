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

    }

}
