using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DomKultury.Models;


namespace DomKultury.Data
{
    public class WydarzeniaContext : DbContext
    {
        public WydarzeniaContext(DbContextOptions options) : base(options) { }
        public DbSet<Wydarzenie> Wydarzenie { get; set; }
    }

}
