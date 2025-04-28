using Microsoft.AspNetCore.Mvc;
using DomKultury.Models;
using System;
using System.Collections.Generic;

namespace DomKultury.Controllers
{
    [Route("Zajecia")]
    public class ZajeciaController : Controller
    {
        // GET: Zajecia
        [HttpGet("")]
        public IActionResult Index()
        {
            var zajecia = new List<Zajecie>
            {
                new Zajecie
                {
                    Id = 1,
                    Nazwa = "Zajęcia Taneczne",
                    Opis = "Zajęcia taneczne dla wszystkich grup wiekowych",
                    Termin = DateTime.Parse("2023-10-23 18:00"),
                    Lokalizacja = "Sala A",
                    Cena = 50.00m,
                    MaksymalnaLiczbaUczestnikow = 20,
                    InstruktorId = 1
                },
                new Zajecie
                {
                    Id = 2,
                    Nazwa = "Plastyka dla dzieci",
                    Opis = "Zajęcia plastyczne dla dzieci w wieku 6-12 lat",
                    Termin = DateTime.Parse("2023-10-24 16:00"),
                    Lokalizacja = "Sala B",
                    Cena = 40.00m,
                    MaksymalnaLiczbaUczestnikow = 15,
                    InstruktorId = 2
                }
            };

            // Przekazanie listy do widoku
            return View(zajecia);
        }

        // GET: Zajecia/Zapisz/1
        [HttpGet("Zapisz/{id}")]
        public IActionResult Zapisz(int id)
        {
            var zajecie = new Zajecie
            {
                Id = id,
                Nazwa = "Zajęcia (przykład)", // Można podmienić, jeśli masz dane z bazy
                Opis = "Opis zajęć",
                Termin = DateTime.Now,
                Lokalizacja = "Sala A",
                Cena = 50.00m,
                MaksymalnaLiczbaUczestnikow = 20,
                InstruktorId = 1
            };

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
            if (ModelState.IsValid)
            {
                TempData["Komunikat"] = "Zostałeś zapisany na zajęcia!";
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
