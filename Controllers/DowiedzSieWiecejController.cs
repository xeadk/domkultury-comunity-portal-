using DomKultury.Data;
using DomKultury.Models;
using Microsoft.AspNetCore.Mvc;

namespace DomKultury.Controllers
{
    public class DowiedzSieWiecejController : Controller
    {

        private readonly WydarzeniaContext _context;

        public DowiedzSieWiecejController(WydarzeniaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var dane = _context.Informacja.ToList();

            var viewModel = new DowiedzSieWiecejViewModel
            {
                Informacja = dane
            };

            return View(viewModel);
        }

    }
}
