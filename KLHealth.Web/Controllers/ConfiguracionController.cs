using KLHealth.Web.Data;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KLHealth.Web.Controllers
{
    [Authorize(Roles = "Paciente")]
    public class ConfiguracionController : Controller
    {
        private readonly KLHealthDbContext _context;

        public ConfiguracionController(KLHealthDbContext context)
        {
            _context = context;
        }

        // GET: Configuracion
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Configuración";

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var usuario = await _context.Usuarios
                .Include(u => u.Paciente)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario == null)
            {
                return NotFound();
            }

            var model = new ConfiguracionViewModel
            {
                UsuarioId = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                NumeroIdentificacion = usuario.Paciente?.NumeroIdentificacion
            };

            return View(model);
        }

        // POST: Configuracion/ActualizarPreferencias
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarPreferencias(ConfiguracionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            // Nota: En una implementación real, aquí guardaríamos las preferencias en la BD
            // Por ahora, solo mostramos un mensaje de éxito

            TempData["Success"] = "Preferencias actualizadas correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}