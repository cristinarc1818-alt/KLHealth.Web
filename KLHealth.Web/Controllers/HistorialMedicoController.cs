using KLHealth.Web.Data;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KLHealth.Web.Controllers
{
    [Authorize(Roles = "Paciente")]
    public class HistorialMedicoController : Controller
    {
        private readonly KLHealthDbContext _context;

        public HistorialMedicoController(KLHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string ano = "Todos")
        {
            ViewData["Title"] = "Historial Médico";

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (paciente == null)
            {
                return NotFound();
            }

            var todos = await _context.HistorialesMedicos
                .Include(h => h.Medico).ThenInclude(m => m!.Usuario)
                .Include(h => h.Medico).ThenInclude(m => m!.Especialidad)
                .Where(h => h.PacienteId == paciente.Id)
                .OrderByDescending(h => h.FechaConsulta)
                .ToListAsync();

            // Obtener años únicos
            var anosDisponibles = todos.Select(h => h.FechaConsulta.Year).Distinct().OrderByDescending(y => y).ToList();
            if (!anosDisponibles.Contains(DateTime.Now.Year))
            {
                anosDisponibles.Insert(0, DateTime.Now.Year);
            }

            // Filtrar por año si no es "Todos"
            var registrosFiltrados = todos;
            if (ano != "Todos" && int.TryParse(ano, out int anoInt))
            {
                registrosFiltrados = todos.Where(h => h.FechaConsulta.Year == anoInt).ToList();
            }

            // Agrupar por Mes Año
            var agrupados = registrosFiltrados
                .GroupBy(h => h.FechaConsulta.ToString("MMMM yyyy", new System.Globalization.CultureInfo("es-ES")).ToUpper())
                .ToDictionary(g => g.Key, g => g.ToList());

            var model = new HistorialViewModel
            {
                Registros = registrosFiltrados,
                AnoSeleccionado = ano,
                TotalRegistros = registrosFiltrados.Count,
                RegistrosAgrupados = agrupados,
                AnosDisponibles = anosDisponibles
            };

            return View(model);
        }
    }
}