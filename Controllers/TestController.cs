using DomKultury.Data;
using DomKultury.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DomKultury.Controllers
{
    public class TestController : Controller
    {
        private readonly WydarzeniaContext _context;

        public TestController(WydarzeniaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new TestViewModel
            {
                Zajecia = _context.Zajecie.Include(z => z.Instruktor).Include(z => z.Uczestnicy).ToList(),
                Instruktorzy = _context.Instruktor.Include(i => i.Zajecia).ToList(),
                Uczestnicy = _context.Uczestnik.Include(u => u.Zajecia).ToList(),
                Kategorie = _context.Kategoria.Include(k => k.Wydarzenia).ToList(),
                Wydarzenia = _context.Wydarzenie.Include(w => w.Kategoria).ToList()
            };

            return View(viewModel);
        }
    }

    public class TestViewModel
    {
        public List<Zajecie> Zajecia { get; set; }
        public List<Instruktor> Instruktorzy { get; set; }
        public List<Uczestnik> Uczestnicy { get; set; }
        public List<Kategoria> Kategorie { get; set; }
        public List<Wydarzenie> Wydarzenia { get; set; }
    }
}
