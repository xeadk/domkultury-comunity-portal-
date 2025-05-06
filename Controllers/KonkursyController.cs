using DomKultury.Models;
using Microsoft.AspNetCore.Mvc;

public class KonkursyController : Controller
{
    public IActionResult Index()
    {
        var konkursy = new List<Konkurs>
        {
            new Konkurs
            {
                Id = 1,
                Nazwa = "Konkurs Fotograficzny",
                Opis = "Konkurs na najlepsze zdjęcie przyrody.",
                Termin = DateTime.Now.AddDays(30),
                Organizator = "Dom Kultury",
                Cena = 50,
                ObrazekUrl = "/images/konkurs1.jpg"
            },
            // Dodaj więcej konkursów tutaj
        };

        return View(konkursy);
    }

    public IActionResult Zapisz(int id)
    {
        // Tutaj możesz dodać logikę zapisu do konkursu
        TempData["Komunikat"] = "Zarejestrowano na konkurs!";
        return RedirectToAction("Index");
    }
}
