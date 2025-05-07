using Microsoft.AspNetCore.Mvc;
using DomKultury.Models;
using DomKultury.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System;

namespace DomKultury.Controllers
{
    [Route("Zajecia")]
    public class ZajeciaController : Controller
    {
        private readonly WydarzeniaContext _context;
        private readonly IWebHostEnvironment _env;

        public ZajeciaController(WydarzeniaContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Zajecia
        [HttpGet("")]
        public IActionResult Index()
        {
            var zajecia = _context.Zajecie
                .Include(z => z.Instruktor)
                .ToList();

            return View(zajecia);
        }

        // GET: Zajecia/Create
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.Instruktorzy = _context.Instruktor.ToList();
            return View();
        }

        // POST: Zajecia/Create
        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Create(Zajecie model, IFormFile Obrazek)
        {
            Console.WriteLine(">>> Create POST start");
            Console.WriteLine("Model valid: " + ModelState.IsValid);
            Console.WriteLine("InstruktorId: " + model.InstruktorId);
            Console.WriteLine("Nazwa: " + model.Nazwa);

            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"Błąd w polu {entry.Key}: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                if (Obrazek != null && Obrazek.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Obrazek.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Obrazek.CopyToAsync(fileStream);
                    }

                    model.ObrazekUrl = "/uploads/" + uniqueFileName;
                }

                _context.Zajecie.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Instruktorzy = _context.Instruktor.ToList();
            return View(model);
        }


        // GET: Zajecia/Edit/5
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var zajecie = _context.Zajecie.Find(id);
            if (zajecie == null)
                return NotFound();

            ViewBag.Instruktorzy = _context.Instruktor.ToList();
            return View(zajecie);
        }

        // POST: Zajecia/Edit/5
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, Zajecie model, IFormFile? Obrazek)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (Obrazek != null && Obrazek.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Obrazek.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await Obrazek.CopyToAsync(fileStream);
                        }

                        model.ObrazekUrl = "/uploads/" + uniqueFileName;
                    }

                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Zajecie.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction("Index");
            }

            ViewBag.Instruktorzy = _context.Instruktor.ToList();
            return View(model);
        }

        // GET: Zajecia/Delete/5
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var zajecie = _context.Zajecie
                .Include(z => z.Instruktor)
                .FirstOrDefault(z => z.Id == id);
            if (zajecie == null)
                return NotFound();

            return View(zajecie);
        }
        // POST: Zajecia/Delete/5
        [HttpPost("Delete/{id}")]
        [Authorize(Roles = "Admin,Moderator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zajecie = await _context.Zajecie.FindAsync(id);
            if (zajecie != null)
            {
                _context.Zajecie.Remove(zajecie);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
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
