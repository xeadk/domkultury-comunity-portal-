using DomKultury.Data;
using DomKultury.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DomKultury.Controllers
{
    public class AdministracjaController : Controller
    {
        private readonly WydarzeniaContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AdministracjaController(WydarzeniaContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> ManageRoles()
        {
            var users = _userManager.Users.ToList();
            var roles = _roleManager.Roles.ToList();

            var model = new ManageRolesViewModel
            {
                Users = users,
                Roles = roles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(); // Zwróć 404, jeśli użytkownik nie istnieje

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!string.IsNullOrEmpty(roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }

            return RedirectToAction(nameof(ManageRoles)); // Zwróć przekierowanie po zakończeniu
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            await _userManager.DeleteAsync(user);
            // Zwróć zaktualizowany widok, nie przekierowuj
            var users = _userManager.Users.ToList();
            var roles = _roleManager.Roles.ToList();
            var model = new ManageRolesViewModel
            {
                Users = users,
                Roles = roles
            };
            return RedirectToAction(nameof(ManageRoles));
        }


        public IActionResult Index()
        {
            var viewModel = new TestViewModel
            {
                Zajecia = _context.Zajecie.Include(z => z.Instruktor).Include(z => z.Uczestnicy).ToList(),
                Instruktorzy = _context.Instruktor.Include(i => i.Zajecia).ToList(),
                Uczestnicy = _context.Uczestnik.Include(u => u.Zajecia).ToList(),
                Kategorie = _context.Kategoria.Include(k => k.Wydarzenia).ToList(),
                Wydarzenia = _context.Wydarzenie.Include(w => w.Kategoria).ToList()
            };

            return View(viewModel);
        }
    }

    public class TestViewModel
    {
        public List<Zajecie> Zajecia { get; set; }
        public List<Instruktor> Instruktorzy { get; set; }
        public List<Uczestnik> Uczestnicy { get; set; }
        public List<Kategoria> Kategorie { get; set; }
        public List<Wydarzenie> Wydarzenia { get; set; }
    }
    [Route("AdminApi")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly WydarzeniaContext _context;
        public AdminApiController(WydarzeniaContext context) => _context = context;

        [HttpPost("Create/{entityType}")]
        public async Task<IActionResult> Create(string entityType, [FromBody] JsonElement body)
        {
            var entity = JsonSerializer.Deserialize(body.GetRawText(), Type.GetType("DomKultury.Models." + entityType));
            _context.Add(entity);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("Update/{entityType}/{id}")]
        public async Task<IActionResult> Update(string entityType, int id, [FromBody] JsonElement body)
        {
            var type = Type.GetType("DomKultury.Models." + entityType);
            var existing = await _context.FindAsync(type, id);
            if (existing == null) return NotFound();

            var updated = JsonSerializer.Deserialize(body.GetRawText(), type);

            // NIE NADPISUJ Id!
            var idProp = type.GetProperty("Id");
            if (idProp != null && updated != null && existing != null)
            {
                var originalId = idProp.GetValue(existing);
                idProp.SetValue(updated, originalId); // przywróć oryginalne Id na wszelki wypadek
            }

            _context.Entry(existing).CurrentValues.SetValues(updated);
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpDelete("Delete/{entityType}/{id}")]
        public async Task<IActionResult> Delete(string entityType, int id)
        {
            var type = Type.GetType("DomKultury.Models." + entityType);
            var existing = await _context.FindAsync(type, id);
            if (existing == null) return NotFound();

            _context.Remove(existing);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
