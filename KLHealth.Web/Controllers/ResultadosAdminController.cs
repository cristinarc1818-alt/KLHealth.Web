using KLHealth.Web.Data;
using KLHealth.Web.Models.Entities;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KLHealth.Web.Controllers
{
    [Authorize]
    public class ResultadosAdminController : Controller
    {
        private readonly KLHealthDbContext _context;

        public ResultadosAdminController(KLHealthDbContext context)
        {
            _context = context;
        }

        // GET: ResultadosAdmin
        public async Task<IActionResult> Index(string tab = "Todos", int? id = null, string busqueda = "", string fechaDesde = "", string fechaHasta = "")
        {
            ViewData["Title"] = "Resultados Médicos";

            var resultadosQuery = _context.ResultadosMedicos
                .Include(r => r.Medico).ThenInclude(m => m!.Usuario)
                .Include(r => r.Paciente).ThenInclude(p => p!.Usuario)
                .AsQueryable();

            // Filtro por pestaña
            if (!string.IsNullOrEmpty(tab) && tab != "Todos")
            {
                resultadosQuery = resultadosQuery.Where(r => r.Tipo.Contains(tab));
            }

            // Filtro por búsqueda de texto
            if (!string.IsNullOrEmpty(busqueda))
            {
                resultadosQuery = resultadosQuery.Where(r =>
                    r.NombreExamen.Contains(busqueda) ||
                    (r.Paciente != null && r.Paciente.Usuario != null && r.Paciente.Usuario.NombreCompleto.Contains(busqueda)) ||
                    (r.Medico != null && r.Medico.Usuario != null && r.Medico.Usuario.NombreCompleto.Contains(busqueda)));
            }

            // Filtro por rango de fechas
            if (!string.IsNullOrEmpty(fechaDesde) && DateTime.TryParse(fechaDesde, out var desde))
            {
                resultadosQuery = resultadosQuery.Where(r => r.Fecha >= desde);
            }
            if (!string.IsNullOrEmpty(fechaHasta) && DateTime.TryParse(fechaHasta, out var hasta))
            {
                resultadosQuery = resultadosQuery.Where(r => r.Fecha <= hasta.AddDays(1));
            }

            resultadosQuery = resultadosQuery.OrderByDescending(r => r.Fecha);
            var resultados = await resultadosQuery.ToListAsync();

            ResultadoMedico? resultadoSeleccionado = null;
            if (id.HasValue)
            {
                resultadoSeleccionado = resultados.FirstOrDefault(i => i.Id == id.Value);
            }
            else if (resultados.Any())
            {
                resultadoSeleccionado = resultados.First();
            }

            var model = new AdminResultadosViewModel
            {
                Resultados = resultados,
                TabActual = tab,
                ResultadoSeleccionado = resultadoSeleccionado,
                TotalResultados = resultados.Count,
                TotalPendientes = resultados.Count(i => i.Pendiente)
            };

            return View(model);
        }

        // GET: ResultadosAdmin/Crear
        public async Task<IActionResult> Crear()
        {
            ViewData["Title"] = "Nuevo Resultado";
            var model = new CrearResultadoViewModel();
            await CargarListas(model);
            return View(model);
        }

        // POST: ResultadosAdmin/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearResultadoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarListas(model);
                return View(model);
            }

            var resultado = new ResultadoMedico
            {
                PacienteId = model.PacienteId,
                MedicoId = model.MedicoId,
                Tipo = model.Tipo,
                NombreExamen = model.NombreExamen,
                Descripcion = model.Descripcion,
                Fecha = model.Fecha,
                Pendiente = true,
                ArchivoUrl = model.ArchivoUrl
            };

            _context.ResultadosMedicos.Add(resultado);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Resultado creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: ResultadosAdmin/MarcarFinalizado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarFinalizado(int id)
        {
            var resultado = await _context.ResultadosMedicos.FindAsync(id);
            if (resultado == null) return NotFound();

            resultado.Pendiente = false;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Resultado marcado como finalizado correctamente.";
            return RedirectToAction(nameof(Index), new { id = id });
        }

        // POST: ResultadosAdmin/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _context.ResultadosMedicos.FindAsync(id);
            if (resultado == null) return NotFound();

            _context.ResultadosMedicos.Remove(resultado);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Resultado eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para cargar listas (CORREGIDO)
        private async Task CargarListas(CrearResultadoViewModel model)
        {
            // Cargar datos por separado
            var pacientes = await _context.Pacientes.ToListAsync();
            var usuariosPacientes = await _context.Usuarios.ToListAsync();
            var medicos = await _context.Medicos.ToListAsync();
            var usuariosMedicos = await _context.Usuarios
                .Where(u => u.RolId == 2) // Rol de Médico
                .ToListAsync();

            // Join manual en memoria para Pacientes
            model.Pacientes = pacientes
                .Join(usuariosPacientes,
                      p => p.UsuarioId,
                      u => u.Id,
                      (p, u) => new SelectItem
                      {
                          Id = p.Id,
                          Nombre = $"{u.NombreCompleto} ({p.NumeroIdentificacion})"
                      })
                .OrderBy(p => p.Nombre)
                .ToList();

            // Join manual en memoria para Médicos
            model.Medicos = medicos
                .Join(usuariosMedicos,
                      m => m.UsuarioId,
                      u => u.Id,
                      (m, u) => new SelectItem
                      {
                          Id = m.Id,
                          Nombre = $"{m.Titulo} {u.NombreCompleto}"
                      })
                .OrderBy(m => m.Nombre)
                .ToList();

            // Tipos de examen
            model.TiposExamen = new List<SelectItem>
            {
                new SelectItem { Id = 1, Nombre = "Laboratorio" },
                new SelectItem { Id = 2, Nombre = "Radiología" },
                new SelectItem { Id = 3, Nombre = "Electrocardiograma" },
                new SelectItem { Id = 4, Nombre = "Rayos X" },
                new SelectItem { Id = 5, Nombre = "Resonancia Magnética" },
                new SelectItem { Id = 6, Nombre = "Tomografía" },
                new SelectItem { Id = 7, Nombre = "Ultrasonido" },
                new SelectItem { Id = 8, Nombre = "Consulta" }
            };
        }
    }
}