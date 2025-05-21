using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DomKultury.Data;
using DomKultury.Models;
using Microsoft.AspNetCore.Authorization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure; 
using System.IO;

namespace DomKultury.Controllers
{
    [Route("Wydarzenia")]
    public class WydarzeniaController : Controller
    {
        private readonly WydarzeniaContext _context;
        private readonly IWebHostEnvironment _env;

        public WydarzeniaController(WydarzeniaContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Wydarzenia
        [HttpGet("")]
        public IActionResult Index(int? categoryId)
        {
            var wydarzeniaQuery = _context.Wydarzenie
                .Include(w => w.Kategoria)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                wydarzeniaQuery = wydarzeniaQuery.Where(w => w.KategoriaId == categoryId.Value);
            }

            var wydarzenia = wydarzeniaQuery.ToList();

            ViewBag.Kategorie = _context.Kategoria.ToList();

            return View(wydarzenia);
        }

        // GET: Wydarzenia/Create
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.Kategorie = _context.Kategoria.ToList();
            return View(new Wydarzenie());
        }

        // POST: Wydarzenia/Create
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(Wydarzenie model, IFormFile? Obrazek, string? KategoriaNazwa)
        {
            if (!string.IsNullOrWhiteSpace(KategoriaNazwa))
            {
                var istniejąca = _context.Kategoria.FirstOrDefault(k => k.Nazwa == KategoriaNazwa);
                if (istniejąca != null)
                {
                    model.KategoriaId = istniejąca.Id;
                }
                else
                {
                    var nowa = new Kategoria { Nazwa = KategoriaNazwa };
                    _context.Kategoria.Add(nowa);
                    await _context.SaveChangesAsync();
                    model.KategoriaId = nowa.Id;
                }
            }

            if (!string.IsNullOrWhiteSpace(model.ObrazekUrl))
            {
            }
            else if (Obrazek != null && Obrazek.Length > 0)
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

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState.IsValid == false. Błędy walidacji:");
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Pole: {entry.Key}, Błąd: {error.ErrorMessage}");
                    }
                }

                ViewBag.Kategorie = _context.Kategoria.ToList();
                return View(model);
            }

            _context.Wydarzenie.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Wydarzenia/Edit/5
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var wydarzenie = _context.Wydarzenie.Find(id);
            if (wydarzenie == null)
                return NotFound();

            ViewBag.Kategorie = _context.Kategoria.ToList();
            return View(wydarzenie);
        }

        // POST: Wydarzenia/Edit/5
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, Wydarzenie model, IFormFile? Obrazek)
        {
            if (id != model.Id)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(model.ObrazekUrl))
            {
            }
            else if (Obrazek != null && Obrazek.Length > 0)
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

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Wydarzenie.Any(w => w.Id == id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction("Index");
            }

            ViewBag.Kategorie = _context.Kategoria.ToList();
            return View(model);
        }

        // GET: Wydarzenia/Delete/5
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var wydarzenie = _context.Wydarzenie
                .Include(w => w.Kategoria)
                .FirstOrDefault(w => w.Id == id);
            if (wydarzenie == null)
                return NotFound();

            return View(wydarzenie);
        }

        // POST: Wydarzenia/Delete/5
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wydarzenie = await _context.Wydarzenie.FindAsync(id);
            if (wydarzenie != null)
            {
                _context.Wydarzenie.Remove(wydarzenie);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // GET: Wydarzenia/Szczegoly/5
        [HttpGet("Szczegoly/{id}")]
        public IActionResult Szczegoly(int id)
        {
            var wydarzenie = _context.Wydarzenie
                .Include(w => w.Kategoria)
                .FirstOrDefault(w => w.Id == id);

            if (wydarzenie == null)
                return NotFound();

            return View(wydarzenie);
        }


        [HttpPost("GenerujPDF")]
        [ValidateAntiForgeryToken]
        public IActionResult GenerujPDF(int wydarzenieId)
        {
            var wydarzenie = _context.Wydarzenie
                .Include(w => w.Kategoria)
                .FirstOrDefault(w => w.Id == wydarzenieId);

            if (wydarzenie == null)
                return NotFound();

            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Size(PageSizes.A4);

                        page.Content()
                            .Column(col =>
                            {
                                col.Item().Container().PaddingBottom(20).AlignCenter().Text("Potwierdzenie rezerwacji")
                                    .FontSize(20).Bold().Underline();

                                col.Item().Text($"Nazwa: {wydarzenie.Nazwa}").FontSize(14);
                                col.Item().Text($"Kategoria: {wydarzenie.Kategoria?.Nazwa}").FontSize(14);
                                col.Item().Text($"Data: {wydarzenie.Data:dd.MM.yyyy}").FontSize(14);
                                col.Item().Text($"Lokalizacja: {wydarzenie.Lokalizacja}").FontSize(14);
                                col.Item().Text($"Organizator: {wydarzenie.Organizator}").FontSize(14);
                                col.Item().Text($"Status: {(wydarzenie.Status ? "Zaplanowane" : "Odwołane")}").FontSize(14);
                            });
                    });
                });

                byte[] pdfBytes = document.GeneratePdf();
                return File(pdfBytes, "application/pdf", $"Rezerwacja_{wydarzenie.Nazwa}.pdf");
            }
            catch (Exception ex)
            {
                return Content($"❌ Błąd generowania PDF: {ex.Message}");
            }
        }


    }
}
