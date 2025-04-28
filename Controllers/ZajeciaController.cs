using Microsoft.AspNetCore.Mvc;
using DomKultury.Models;
using DomKultury.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DomKultury.Controllers
{
    [Route("Zajecia")]
    public class ZajeciaController : Controller
    {
        private readonly WydarzeniaContext _context;

        public ZajeciaController(WydarzeniaContext context)
        {
            _context = context;
        }

        // GET: Zajecia
        [HttpGet("")]
        public IActionResult Index()
        {
            var zajecia = _context.Zajecie.ToList(); // <-- teraz pobieramy z bazy
            return View(zajecia);
        }

        // GET: Zajecia/Zapisz/1
        [HttpGet("Zapisz/{id}")]
        public IActionResult Zapisz(int id)
        {
            var zajecie = _context.Zajecie.FirstOrDefault(z => z.Id == id);
            if (zajecie == null)
            {
                return NotFound();
            }

            var model = new ZapisViewModel
            {
                ZajecieId = zajecie.Id
            };

            return View(model);
        }

        // POST: Zajecia/Zapisz/1
        [HttpPost("Zapisz/{id}")]
        public IActionResult Zapisz(int id, ZapisViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var zajecie = _context.Zajecie
                .Include(z => z.Uczestnicy)
                .FirstOrDefault(z => z.Id == id);

            if (zajecie == null)
            {
                return NotFound();
            }

            // Szukamy, czy istnieje już uczestnik o tym samym emailu
            var istniejącyUczestnik = _context.Uczestnik
                .Include(u => u.Zajecia)
                .FirstOrDefault(u => u.Email == model.Email);

            if (istniejącyUczestnik != null)
            {
                // Sprawdzamy, czy już zapisany na to zajęcie
                bool juzZapisany = istniejącyUczestnik.Zajecia.Any(z => z.Id == id);
                if (juzZapisany)
                {
                    ModelState.AddModelError(string.Empty, "Już jesteś zapisany na te zajęcia.");
                    return View(model);
                }

                // Jeśli nie zapisany, dodajemy go do zajęć
                istniejącyUczestnik.Zajecia.Add(zajecie);
                _context.SaveChanges();

                TempData["Komunikat"] = "Zostałeś zapisany na zajęcia!";
                return RedirectToAction("Index");
            }

            // Jeśli uczestnik nie istnieje, tworzymy nowego
            var uczestnik = new Uczestnik
            {
                Imie = model.Imie,
                Nazwisko = model.Nazwisko,
                Email = model.Email,
                NumerTelefonu = model.NumerTelefonu,
                DataRejestracji = DateTime.Now,
                Zajecia = new List<Zajecie> { zajecie }
            };

            _context.Uczestnik.Add(uczestnik);
            _context.SaveChanges();

            TempData["Komunikat"] = "Zostałeś zapisany na zajęcia!";
            return RedirectToAction("Index");
        }

    }
}
