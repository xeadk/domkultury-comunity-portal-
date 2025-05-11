using DomKultury.Data;
using DomKultury.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DomKultury.Controllers
{
    public class WydarzeniaController : Controller
    {
        private readonly WydarzeniaContext _context;

        public WydarzeniaController(WydarzeniaContext context)
        {
            _context = context;
        }

        // GET: /Wydarzenia
        public async Task<IActionResult> Index(int? categoryId)
        {
            // Tworzymy zapytanie do bazy, aby uwzględnić wydarzenia oraz kategorię
            var wydarzeniaQuery = _context.Wydarzenie
                .Include(w => w.Kategoria)
                .AsQueryable(); // Używamy AsQueryable(), aby móc dynamicznie dodawać warunki

            // Filtrowanie po kategorii, jeśli id kategorii jest podane
            if (categoryId.HasValue)
            {
                wydarzeniaQuery = wydarzeniaQuery.Where(w => w.KategoriaId == categoryId.Value);
            }

            // Pobranie wydarzeń, które spełniają warunki
            var wydarzenia = await wydarzeniaQuery.ToListAsync();

            // Pobranie listy kategorii do wyświetlenia w dropdownie
            ViewBag.Kategorie = await _context.Kategoria.ToListAsync();

            // Przekazanie listy wydarzeń i kategorii do widoku
            return View(wydarzenia);
        }
    }
}
