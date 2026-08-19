using KLHealth.Web.Data;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // ← IMPORTANTE: Agregado para obtener el ID del usuario

namespace KLHealth.Web.Controllers
{
    [Authorize]
    public class PerfilAdminController : Controller
    {
        private readonly KLHealthDbContext _context;

        public PerfilAdminController(KLHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Mi Perfil";

            // 1. OBTENER EL ID DEL USUARIO LOGUEADO (Forma más segura en .NET)
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Si por alguna razón no está en las claims, usamos el Identity.Name como respaldo
            if (string.IsNullOrEmpty(userIdString))
            {
                var fallbackName = User.Identity?.Name;
                var usuarioFallback = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == fallbackName || u.NombreCompleto == fallbackName);
                if (usuarioFallback == null)
                {
                    // DEBUG: Si llega aquí, nos dirá qué está buscando exactamente
                    return Content($"ERROR: No se pudo identificar al usuario. Identity.Name = '{fallbackName}'");
                }
                userIdString = usuarioFallback.Id.ToString();
            }

            // 2. CONVERTIR A INT Y BUSCAR EL USUARIO
            if (int.TryParse(userIdString, out int userId))
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (usuario == null) return NotFound("Usuario no encontrado en la base de datos.");

                var model = new PerfilAdminViewModel
                {
                    Id = usuario.Id,
                    NombreCompleto = usuario.NombreCompleto,
                    Email = usuario.Email,
                    Telefono = usuario.Telefono,
                    Rol = usuario.Rol?.Nombre ?? "Usuario",
                    EstaActivo = usuario.EstaActivo,
                    FechaRegistro = usuario.FechaRegistro
                };

                // 3. Si es Médico (RolId = 2), cargar datos adicionales
                if (usuario.RolId == 2)
                {
                    var medico = await _context.Medicos
                        .Include(m => m.Especialidad)
                        .FirstOrDefaultAsync(m => m.UsuarioId == usuario.Id);

                    if (medico != null)
                    {
                        model.Especialidad = medico.Especialidad?.Nombre;
                        model.FotoPerfilUrl = medico.FotoPerfilUrl;
                    }
                }

                return View(model);
            }

            return NotFound("No se pudo obtener el ID del usuario.");
        }

        public async Task<IActionResult> Editar()
        {
            ViewData["Title"] = "Editar Perfil";
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdString, out int userId))
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == userId);
                if (usuario == null) return NotFound();

                var model = new PerfilAdminViewModel
                {
                    Id = usuario.Id,
                    NombreCompleto = usuario.NombreCompleto,
                    Email = usuario.Email,
                    TelefonoEdit = usuario.Telefono
                };

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(PerfilAdminViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = await _context.Usuarios.FindAsync(model.Id);
            if (usuario == null) return NotFound();

            usuario.NombreCompleto = model.NombreCompleto;
            usuario.Telefono = model.TelefonoEdit;
            usuario.FechaUltimaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Perfil actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}