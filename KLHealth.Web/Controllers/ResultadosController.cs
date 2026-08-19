using KLHealth.Web.Data;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KLHealth.Web.Controllers
{
    [Authorize(Roles = "Paciente")]
    public class ResultadosController : Controller
    {
        private readonly KLHealthDbContext _context;

        public ResultadosController(KLHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string filtro = "Todos", string tab = "Todos")
        {
            ViewData["Title"] = "Resultados Médicos";

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (paciente == null)
            {
                return NotFound();
            }

            var todos = await _context.ResultadosMedicos
                .Include(r => r.Medico)
                .ThenInclude(m => m!.Usuario)
                .Where(r => r.PacienteId == paciente.Id)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();

            var model = new ResultadosViewModel
            {
                TodosLosResultados = todos,
                FiltroActual = filtro,
                TabActual = tab,
                TotalResultados = todos.Count,
                TotalDisponibles = todos.Count(r => !r.Pendiente),
                TotalPendientes = todos.Count(r => r.Pendiente),
                TotalRecientes = todos.Count(r => r.Fecha >= DateTime.Now.AddDays(-30)),
                TotalMarcados = todos.Count(r => r.Marcado),
                UltimoResultado = todos.FirstOrDefault(r => !r.Pendiente),
                ResultadosEsteMes = todos.Count(r => r.Fecha.Month == DateTime.Now.Month && r.Fecha.Year == DateTime.Now.Year)
            };

            // Aplicar filtro de estado
            if (filtro == "Disponibles")
            {
                model.TodosLosResultados = todos.Where(r => !r.Pendiente).ToList();
            }
            else if (filtro == "Pendientes")
            {
                model.TodosLosResultados = todos.Where(r => r.Pendiente).ToList();
            }

            // Aplicar tab
            if (tab == "Recientes")
            {
                model.TodosLosResultados = todos.Where(r => r.Fecha >= DateTime.Now.AddDays(-30)).ToList();
            }
            else if (tab == "Marcados")
            {
                model.TodosLosResultados = todos.Where(r => r.Marcado).ToList();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Marcar(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (paciente == null) return NotFound();

            var resultado = await _context.ResultadosMedicos
                .FirstOrDefaultAsync(r => r.Id == id && r.PacienteId == paciente.Id);

            if (resultado == null) return NotFound();

            resultado.Marcado = !resultado.Marcado;
            await _context.SaveChangesAsync();

            TempData["Success"] = resultado.Marcado ? "Resultado marcado" : "Resultado desmarcado";
            return RedirectToAction(nameof(Index));
        }

        // GET: Resultados/Detalles/5
        public async Task<IActionResult> Detalles(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (paciente == null) return NotFound();

            // VALIDACIÓN DE SEGURIDAD: Solo obtiene el resultado si pertenece al paciente logueado
            var resultado = await _context.ResultadosMedicos
                .Include(r => r.Medico).ThenInclude(m => m!.Usuario)
                .Include(r => r.Medico).ThenInclude(m => m!.Especialidad)
                .FirstOrDefaultAsync(r => r.Id == id && r.PacienteId == paciente.Id);

            if (resultado == null) return NotFound();

            ViewData["Title"] = "Detalle de Resultado";
            return View(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Descargar(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (paciente == null) return NotFound();

            var resultado = await _context.ResultadosMedicos
                .FirstOrDefaultAsync(r => r.Id == id && r.PacienteId == paciente.Id);

            if (resultado == null || resultado.Pendiente) return NotFound();

            // Simular descarga (en producción sería un archivo real)
            TempData["Success"] = $"Descargando: {resultado.NombreExamen ?? resultado.Tipo}";
            return RedirectToAction(nameof(Index));
        }
    }

}