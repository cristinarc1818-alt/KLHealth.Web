using KLHealth.Web.Data;
using KLHealth.Web.Models.Entities;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KLHealth.Web.Controllers
{
    [Authorize(Roles = "Paciente")]
    public class NotificacionesController : Controller
    {
        private readonly KLHealthDbContext _context;

        public NotificacionesController(KLHealthDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Notificaciones y Recordatorios";

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (paciente == null)
            {
                return NotFound();
            }

            var notificaciones = await _context.Notificaciones
                .Where(n => n.PacienteId == paciente.Id)
                .OrderByDescending(n => n.Fecha)
                .ToListAsync();

            return View(notificaciones);
        }

      
        public IActionResult Crear()
        {
            ViewData["Title"] = "Crear Recordatorio";
            return View(new NotificacionViewModel());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(NotificacionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

                if (paciente == null)
                {
                    return NotFound();
                }

                var notificacion = new Notificacion
                {
                    PacienteId = paciente.Id,
                    Titulo = model.Titulo,
                    Mensaje = model.Mensaje,
                    Icono = model.Icono,
                    Color = model.Color,
                    Fecha = DateTime.Now,
                    Leida = false
                };

                _context.Notificaciones.Add(notificacion);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Recordatorio creado exitosamente y guardado en la base de datos.";
                return RedirectToAction(nameof(Index));
            }

           
            return View(model);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarLeida(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (paciente == null) return NotFound();

            var notificacion = await _context.Notificaciones
                .FirstOrDefaultAsync(n => n.Id == id && n.PacienteId == paciente.Id);

            if (notificacion == null) return NotFound();

            notificacion.Leida = !notificacion.Leida;
            await _context.SaveChangesAsync();

            TempData["Success"] = notificacion.Leida
                ? "Notificación marcada como leída."
                : "Notificación marcada como no leída.";

            return RedirectToAction(nameof(Index));
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (paciente == null) return NotFound();

            var notificacion = await _context.Notificaciones
                .FirstOrDefaultAsync(n => n.Id == id && n.PacienteId == paciente.Id);

            if (notificacion == null) return NotFound();

            _context.Notificaciones.Remove(notificacion);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Registro eliminado permanentemente de la base de datos.";
            return RedirectToAction(nameof(Index));
        }
    }
}