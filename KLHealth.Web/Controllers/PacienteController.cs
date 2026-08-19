
using KLHealth.Web.Data;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KLHealth.Web.Controllers
{
    [Authorize(Roles = "Paciente")]
    public class PacienteController : Controller
    {
        private readonly KLHealthDbContext _context;

        public PacienteController(KLHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = "Inicio";

            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var nombreUsuario = User.Identity?.Name ?? "Paciente";

            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.UsuarioId == userId);

            var model = new PacienteDashboardViewModel
            {
                NombrePaciente = nombreUsuario,
                IdPaciente = paciente?.NumeroIdentificacion ?? "N/A"
            };

            if (paciente != null)
            {
                // ============================================================
                // 1. PRÓXIMA CITA
                // ============================================================

                var proximaCita = await _context.Citas
                    .Include(c => c.Medico)
                        .ThenInclude(m => m!.Especialidad)
                    .Include(c => c.Medico)
                        .ThenInclude(m => m!.Usuario)
                    .Where(c =>
                        c.PacienteId == paciente.Id &&
                        c.FechaHoraInicio >= DateTime.Now &&
                        c.Estado != "Cancelada")
                    .OrderBy(c => c.FechaHoraInicio)
                    .FirstOrDefaultAsync();

                model.ProximaCita = proximaCita;


                if (proximaCita?.Medico != null)
                {
                    // ========================================================
                    // LIMPIAR EL NOMBRE DEL MÉDICO
                    // ========================================================

                    var titulo = proximaCita.Medico.Titulo?.Trim() ?? "";
                    var nombre = proximaCita.Medico.Usuario?.NombreCompleto?.Trim() ?? "";

                    // --------------------------------------------------------
                    // Si NombreCompleto ya contiene Dr./Dra., lo eliminamos.
                    // --------------------------------------------------------

                    while (
                        nombre.StartsWith("Dr.", StringComparison.OrdinalIgnoreCase) ||
                        nombre.StartsWith("Dra.", StringComparison.OrdinalIgnoreCase) ||
                        nombre.StartsWith("Dr ", StringComparison.OrdinalIgnoreCase) ||
                        nombre.StartsWith("Dra ", StringComparison.OrdinalIgnoreCase) ||
                        nombre.StartsWith("Doctor ", StringComparison.OrdinalIgnoreCase) ||
                        nombre.StartsWith("Doctora ", StringComparison.OrdinalIgnoreCase)
                    )
                    {
                        if (nombre.StartsWith("Dra.", StringComparison.OrdinalIgnoreCase))
                        {
                            nombre = nombre.Substring(4).Trim();
                        }
                        else if (nombre.StartsWith("Dr.", StringComparison.OrdinalIgnoreCase))
                        {
                            nombre = nombre.Substring(3).Trim();
                        }
                        else if (nombre.StartsWith("Dra ", StringComparison.OrdinalIgnoreCase))
                        {
                            nombre = nombre.Substring(4).Trim();
                        }
                        else if (nombre.StartsWith("Dr ", StringComparison.OrdinalIgnoreCase))
                        {
                            nombre = nombre.Substring(3).Trim();
                        }
                        else if (nombre.StartsWith("Doctora ", StringComparison.OrdinalIgnoreCase))
                        {
                            nombre = nombre.Substring(8).Trim();
                        }
                        else if (nombre.StartsWith("Doctor ", StringComparison.OrdinalIgnoreCase))
                        {
                            nombre = nombre.Substring(7).Trim();
                        }
                    }


                    // --------------------------------------------------------
                    // Normalizamos el título.
                    // --------------------------------------------------------

                    if (titulo.Contains("Dra", StringComparison.OrdinalIgnoreCase) ||
                        titulo.Contains("Doctora", StringComparison.OrdinalIgnoreCase))
                    {
                        titulo = "Dra.";
                    }
                    else if (titulo.Contains("Dr", StringComparison.OrdinalIgnoreCase) ||
                             titulo.Contains("Doctor", StringComparison.OrdinalIgnoreCase))
                    {
                        titulo = "Dr.";
                    }
                    else
                    {
                        titulo = "";
                    }


                    // --------------------------------------------------------
                    // Construimos el nombre final.
                    // --------------------------------------------------------

                    model.MedicoNombre = string.IsNullOrEmpty(titulo)
                        ? nombre
                        : $"{titulo} {nombre}";


                    // ========================================================
                    // RESTO DE INFORMACIÓN DEL MÉDICO
                    // ========================================================

                    model.EspecialidadNombre =
                        proximaCita.Medico.Especialidad?.Nombre
                        ?? "Medicina General";

                    model.FotoMedicoUrl =
                        proximaCita.Medico.FotoPerfilUrl
                        ?? "https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=200&h=200&fit=crop&crop=face";

                    model.UbicacionCompleta =
                        $"Campus Principal, {proximaCita.Sala ?? "Consultorio"}";

                    model.FechaHoraFormateada =
                        proximaCita.FechaHoraInicio.ToString(
                            "dd 'de' MMMM, yyyy, hh:mm tt"
                        );
                }


                // ============================================================
                // 2. KPIs REALES
                // ============================================================

                model.TotalResultadosPendientes =
                    await _context.ResultadosMedicos
                        .CountAsync(r =>
                            r.PacienteId == paciente.Id &&
                            r.Pendiente);

                model.TotalRegistrosMedicos =
                    await _context.Citas
                        .CountAsync(c =>
                            c.PacienteId == paciente.Id &&
                            c.Estado == "Completada");

                model.NotificacionesNuevas =
                    await _context.Notificaciones
                        .CountAsync(n =>
                            n.PacienteId == paciente.Id &&
                            !n.Leida);


                // ============================================================
                // 3. ÚLTIMAS 3 NOTIFICACIONES
                // ============================================================

                model.UltimasNotificaciones =
                    await _context.Notificaciones
                        .Where(n => n.PacienteId == paciente.Id)
                        .OrderByDescending(n => n.Fecha)
                        .Take(3)
                        .ToListAsync();
            }

            return View(model);
        }
    }
}
