using Microsoft.AspNetCore.Mvc;
using DomKultury.Models; // Dodane dla klasy Zajecie
using System.Collections.Generic; // Dodane dla List

namespace DomKultury.Controllers
{
    [Route("Zajecia")]
    public class ZajeciaController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            // W tej chwili dane są wprowadzone "na sztywno" (na razie na próbę)
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
                    InstruktorId = 1 // Później połączymy z rzeczywistym instruktorem
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
                    InstruktorId = 2 // Później połączymy z rzeczywistym instruktorem
                }
            };

            // Zwracamy dane do widoku
            return View(zajecia);
        }
    }
}
