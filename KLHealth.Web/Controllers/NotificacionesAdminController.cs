using KLHealth.Web.Data;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KLHealth.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class NotificacionesAdminController : Controller
    {
        private readonly KLHealthDbContext _context;

        public NotificacionesAdminController(KLHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Notificaciones";

            var model = new NotificacionesViewModel();

            // Obtener citas pendientes de hoy
            var citasPendientes = await _context.Citas
                .Include(c => c.Paciente).ThenInclude(p => p!.Usuario)
                .Include(c => c.Medico).ThenInclude(m => m!.Usuario)
                .Where(c => c.Estado == "Pendiente" || c.Estado == "Confirmada")
                .OrderByDescending(c => c.FechaHoraInicio)
                .Take(10)
                .ToListAsync();

            // Obtener resultados nuevos (pendientes de revisión)
            var resultadosNuevos = await _context.ResultadosMedicos
                .Include(r => r.Paciente).ThenInclude(p => p!.Usuario)
                .Include(r => r.Medico).ThenInclude(m => m!.Usuario)
                .Where(r => r.Pendiente)
                .OrderByDescending(r => r.Fecha)
                .Take(10)
                .ToListAsync();

            // Obtener pacientes nuevos (últimos 5 registros)
            var pacientesNuevos = await _context.Pacientes
                .Include(p => p.Usuario)
                .OrderByDescending(p => p.Usuario.FechaRegistro)
                .Take(5)
                .ToListAsync();

            // Construir lista de notificaciones
            var notificaciones = new List<NotificacionItem>();

            // Agregar notificaciones de citas
            foreach (var cita in citasPendientes)
            {
                notificaciones.Add(new NotificacionItem
                {
                    Id = cita.Id,
                    Tipo = "Cita",
                    Titulo = "Cita pendiente de confirmación",
                    Mensaje = $"El paciente {cita.Paciente?.Usuario.NombreCompleto} tiene una cita programada para el {cita.FechaHoraInicio:dd MMM yyyy} a las {cita.FechaHoraInicio:hh:mm tt} con el Dr. {cita.Medico?.Usuario.NombreCompleto}",
                    Fecha = cita.FechaHoraInicio,
                    Leida = false,
                    UrlAccion = $"/AdminCitas/Detalles/{cita.Id}",
                    Icono = "bi-calendar-check",
                    ColorIcono = "#dbeafe",
                    ColorTexto = "var(--blue-700)"
                });
            }

            // Agregar notificaciones de resultados
            foreach (var resultado in resultadosNuevos)
            {
                notificaciones.Add(new NotificacionItem
                {
                    Id = resultado.Id,
                    Tipo = "Resultado",
                    Titulo = "Resultado de examen disponible",
                    Mensaje = $"El examen {resultado.NombreExamen} del paciente {resultado.Paciente?.Usuario.NombreCompleto} está listo para revisión",
                    Fecha = resultado.Fecha,
                    Leida = false,
                    UrlAccion = $"/ResultadosAdmin/Index?id={resultado.Id}",
                    Icono = "bi-file-earmark-check",
                    ColorIcono = "#dcfce7",
                    ColorTexto = "#166534"
                });
            }

            // Agregar notificaciones de pacientes nuevos
            foreach (var paciente in pacientesNuevos)
            {
                notificaciones.Add(new NotificacionItem
                {
                    Id = paciente.Id,
                    Tipo = "Paciente",
                    Titulo = "Nuevo paciente registrado",
                    Mensaje = $"{paciente.Usuario.NombreCompleto} se ha registrado en el sistema",
                    Fecha = paciente.Usuario.FechaRegistro,
                    Leida = false,
                    UrlAccion = $"/AdminPacientes/Detalles/{paciente.Id}",
                    Icono = "bi-person-plus",
                    ColorIcono = "#f3e8ff",
                    ColorTexto = "#7c3aed"
                });
            }

            // Ordenar por fecha (más recientes primero)
            model.Notificaciones = notificaciones
                .OrderByDescending(n => n.Fecha)
                .Take(20)
                .ToList();

            // Calcular contadores
            model.TotalSinLeer = model.Notificaciones.Count(n => !n.Leida);
            model.TotalCitasPendientes = citasPendientes.Count;
            model.TotalResultadosNuevos = resultadosNuevos.Count;

            return View(model);
        }

        // Marcar notificación como leída
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarComoLeida(int id, string tipo)
        {
            // Aquí podrías marcar la notificación como leída en la BD
            // Por ahora solo redirigimos
            TempData["Success"] = "Notificación marcada como leída";
            return RedirectToAction(nameof(Index));
        }

        // Marcar todas como leídas
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarTodasComoLeidas()
        {
            // Aquí podrías actualizar todas las notificaciones como leídas
            TempData["Success"] = "Todas las notificaciones marcadas como leídas";
            return RedirectToAction(nameof(Index));
        }
    }
}