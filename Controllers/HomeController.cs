using System.Diagnostics;
using DomKultury.Data;
using DomKultury.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;

namespace DomKultury.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WydarzeniaContext _context;
        private readonly IOptions<WeatherSettings> _weatherSettings;

        public HomeController(ILogger<HomeController> logger, WydarzeniaContext context, IOptions<WeatherSettings> weatherSettings)
        {
            _logger = logger;
            _context = context;
            _weatherSettings = weatherSettings;
        }

        public async Task<IActionResult> Index()
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

            // api do pogody
            string apiKey = _weatherSettings.Value.ApiKey;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogWarning("Brak klucza API do pobierania pogody.");
            }

            var pogoda = await PobierzPogodeAsync("Bia³ystok", apiKey); // lub inne miasto

            var model = new HomePageViewModel
            {
                Zajecia = nadchodzaceZajecia,
                Wydarzenia = nadchodzaceWydarzenia,
                Pogoda = pogoda
            };

            return View(model);
        }

        private async Task<WeatherInfo> PobierzPogodeAsync(string miasto, string apiKey)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={miasto}&appid={apiKey}&units=metric&lang=pl";

            using var client = new HttpClient();
            var response = await client.GetStringAsync(url);

            var data = JObject.Parse(response);
            return new WeatherInfo
            {
                City = miasto,
                Temperature = (float)data["main"]["temp"],
                Description = (string)data["weather"][0]["description"],
                Icon = (string)data["weather"][0]["icon"]
            };
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
