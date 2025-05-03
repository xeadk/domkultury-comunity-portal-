using System.Diagnostics;
using DomKultury.Data;
using DomKultury.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DomKultury.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WydarzeniaContext _context;

        public HomeController(ILogger<HomeController> logger, WydarzeniaContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var nadchodzaceZajecia = _context.Zajecie
                .OrderBy(z => z.Termin)
                .Take(3)
                .Select(z => new ZajecieWidok
                {
                    Id = z.Id,
                    Nazwa = z.Nazwa,
                    Termin = z.Termin,
                    ObrazekUrl = z.ObrazekUrl,
                    OpisKrotki = z.Opis.Length > 100 ? z.Opis.Substring(0, 100) + "..." : z.Opis
                })
                .ToList();

            var nadchodzaceWydarzenia = _context.Wydarzenie
                .OrderBy(w => w.Data)
                .Take(3)
                .Select(w => new WydarzenieWidok
                {
                    Id = w.Id,
                    Nazwa = w.Nazwa,
                    Data = w.Data,
                    ObrazekUrl = w.ObrazekUrl,
                    OpisKrotki = w.Opis.Length > 100 ? w.Opis.Substring(0, 100) + "..." : w.Opis
                })
                .ToList();

            var model = new HomePageViewModel
            {
                Zajecia = nadchodzaceZajecia,
                Wydarzenia = nadchodzaceWydarzenia
            };

            return View(model);
        }


        public IActionResult SavedInSession()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
