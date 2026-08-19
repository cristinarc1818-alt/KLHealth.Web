using KLHealth.Web.Data;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KLHealth.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class HistorialMedicoAdminController : Controller
    {
        private readonly KLHealthDbContext _context;

        public HistorialMedicoAdminController(KLHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? pacienteId)
        {
            ViewData["Title"] = "Historias Clínicas";
            var model = new HistorialMedicoViewModel();

            // 1. Cargar lista de pacientes para el selector
            var pacientesData = await _context.Pacientes
                .Include(p => p.Usuario)
                .ToListAsync();

            model.PacientesDisponibles = pacientesData
                .Where(p => p.Usuario != null)
                .Select(p => new PacienteSelectorItem
                {
                    Id = p.Id,
                    NombreCompleto = p.Usuario!.NombreCompleto,
                    NumeroIdentificacion = p.NumeroIdentificacion
                })
                .OrderBy(p => p.NombreCompleto)
                .ToList();

            // 2. Si se seleccionó un paciente, cargar sus datos
            if (pacienteId.HasValue)
            {
                model.PacienteSeleccionadoId = pacienteId.Value;

                model.Paciente = await _context.Pacientes
                    .Include(p => p.Usuario)
                    .FirstOrDefaultAsync(p => p.Id == pacienteId.Value);

                if (model.Paciente != null)
                {
                    // Cargar citas
                    model.Citas = await _context.Citas
                        .Include(c => c.Medico).ThenInclude(m => m!.Usuario)
                        .Where(c => c.PacienteId == pacienteId.Value)
                        .OrderByDescending(c => c.FechaHoraInicio)
                        .Take(10)
                        .ToListAsync();

                    // Cargar resultados
                    model.Resultados = await _context.ResultadosMedicos
                        .Include(r => r.Medico).ThenInclude(m => m!.Usuario)
                        .Where(r => r.PacienteId == pacienteId.Value)
                        .OrderByDescending(r => r.Fecha)
                        .Take(10)
                        .ToListAsync();

                    // Cargar alergias (Comentado por seguridad, descomenta si tienes la tabla)
                    // model.Alergias = await _context.Alergias
                    //     .Where(a => a.PacienteId == pacienteId.Value)
                    //     .ToListAsync();
                }
            }

            return View(model);
        }
    }
}