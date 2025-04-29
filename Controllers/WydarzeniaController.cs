using DomKultury.Data;
using DomKultury.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Index()
        {
            var wydarzenia = await _context.Wydarzenie
                .Include(w => w.Kategoria)
                .ToListAsync();

            return View(wydarzenia);
        }
    }
}
