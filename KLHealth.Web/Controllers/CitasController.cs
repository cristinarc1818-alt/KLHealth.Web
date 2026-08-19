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
    public class CitasController : Controller
    {
        private readonly KLHealthDbContext _context;

        public CitasController(KLHealthDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // GET: Citas/Index
        // Lista de citas con filtros y paginación
        // ============================================================
        public async Task<IActionResult> Index(string filtro = "Todas", int pagina = 1)
        {
            ViewData["Title"] = "Mis Citas";

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (paciente == null)
            {
                return NotFound();
            }

            var todasLasCitasBase = _context.Citas
                .Include(c => c.Medico).ThenInclude(m => m!.Usuario)
                .Include(c => c.Medico).ThenInclude(m => m!.Especialidad)
                .Where(c => c.PacienteId == paciente.Id);

            var model = new CitasListViewModel
            {
                FiltroActual = filtro,
                PaginaActual = pagina,
                RegistrosPorPagina = 5,
                TotalCitas = await todasLasCitasBase.CountAsync(),
                TotalPendientes = await todasLasCitasBase.CountAsync(c => c.Estado == "Pendiente"),
                TotalConfirmadas = await todasLasCitasBase.CountAsync(c => c.Estado == "Confirmada"),
                TotalCompletadas = await todasLasCitasBase.CountAsync(c => c.Estado == "Completada"),
                TotalCanceladas = await todasLasCitasBase.CountAsync(c => c.Estado == "Cancelada")
            };

            IQueryable<Cita> queryFiltrada = todasLasCitasBase;
            if (filtro != "Todas")
            {
                queryFiltrada = todasLasCitasBase.Where(c => c.Estado == filtro);
            }

            model.TotalRegistros = await queryFiltrada.CountAsync();
            model.TotalPaginas = (int)Math.Ceiling((double)model.TotalRegistros / model.RegistrosPorPagina);

            if (pagina < 1) pagina = 1;
            if (pagina > model.TotalPaginas && model.TotalPaginas > 0) pagina = model.TotalPaginas;
            model.PaginaActual = pagina;

            model.TodasLasCitas = await queryFiltrada
                .OrderByDescending(c => c.FechaHoraInicio)
                .Skip((model.PaginaActual - 1) * model.RegistrosPorPagina)
                .Take(model.RegistrosPorPagina)
                .ToListAsync();

            return View(model);
        }

        // ============================================================
        // GET: Citas/Agendar
        // NUEVO: parámetro medicoId para preselección
        // ============================================================
        public async Task<IActionResult> Agendar(int? medicoId)
        {
            ViewData["Title"] = "Agendar Cita";

            var model = new CitaViewModel
            {
                MedicoId = medicoId, // ⬅️ Preseleccionar si viene del perfil
                FechaHoraInicio = DateTime.Now.AddHours(1) // Valor por defecto
            };

            model.MedicosDisponibles = await ObtenerMedicosDisponiblesAsync();

            return View(model);
        }

        // ============================================================
        // POST: Citas/Agendar
        // MEJORADO: Validaciones de seguridad
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agendar(CitaViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (paciente == null)
            {
                return NotFound();
            }

            // ⬇️ VALIDACIONES ADICIONALES DE NEGOCIO
            if (ModelState.IsValid)
            {
                // 1. Validar que la fecha no sea en el pasado
                if (model.FechaHoraInicio < DateTime.Now.AddMinutes(-5)) // 5 min de tolerancia
                {
                    ModelState.AddModelError("FechaHoraInicio", "No puede agendar una cita en el pasado.");
                }
                else
                {
                    // 2. Validar que el médico siga disponible
                    var medicoExiste = await _context.Medicos
                        .AnyAsync(m => m.Id == model.MedicoId && m.EstaDisponible);

                    if (!medicoExiste)
                    {
                        ModelState.AddModelError("MedicoId", "El médico seleccionado ya no está disponible.");
                    }
                    else
                    {
                        // 3. Validar conflicto de horarios (médico u otro paciente)
                        var inicioRango = model.FechaHoraInicio.AddMinutes(-30);
                        var finRango = model.FechaHoraInicio.AddMinutes(30);

                        var hayConflictoMedico = await _context.Citas
                            .AnyAsync(c => c.MedicoId == model.MedicoId
                                && c.Estado != "Cancelada"
                                && c.FechaHoraInicio >= inicioRango
                                && c.FechaHoraInicio <= finRango);

                        if (hayConflictoMedico)
                        {
                            ModelState.AddModelError("FechaHoraInicio",
                                "El médico ya tiene una cita programada en este horario. Por favor elija otro.");
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var cita = new Cita
                {
                    PacienteId = paciente.Id,
                    MedicoId = model.MedicoId!.Value,
                    FechaHoraInicio = model.FechaHoraInicio,
                    Tipo = model.Tipo,
                    Sala = model.Tipo == "Virtual" ? "Consulta Virtual" : model.Sala,
                    Motivo = model.Motivo,
                    Estado = "Pendiente"
                };

                _context.Citas.Add(cita);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cita agendada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            // Si falló la validación, recargar datos
            model.MedicosDisponibles = await ObtenerMedicosDisponiblesAsync();
            return View(model);
        }

        // ============================================================
        // GET: Citas/Detalles/5
        // ============================================================
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (paciente == null) return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Medico).ThenInclude(m => m!.Usuario)
                .Include(c => c.Medico).ThenInclude(m => m!.Especialidad)
                .Include(c => c.Paciente).ThenInclude(p => p!.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id && c.PacienteId == paciente.Id);

            if (cita == null) return NotFound();

            return View(cita);
        }

        // ============================================================
        // POST: Citas/Cancelar
        // MEJORADO: valida estado y política de 24h
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (paciente == null) return NotFound();

            var cita = await _context.Citas
                .FirstOrDefaultAsync(c => c.Id == id && c.PacienteId == paciente.Id);

            if (cita == null) return NotFound();

            // ⬇️ VALIDACIONES DE ESTADO
            if (cita.Estado == "Cancelada")
            {
                TempData["Error"] = "Esta cita ya fue cancelada.";
                return RedirectToAction(nameof(Index));
            }

            if (cita.Estado == "Completada")
            {
                TempData["Error"] = "No se puede cancelar una cita ya completada.";
                return RedirectToAction(nameof(Index));
            }

            // ⬇️ VALIDACIÓN DE POLÍTICA DE 24 HORAS
            var horasRestantes = (cita.FechaHoraInicio - DateTime.Now).TotalHours;
            if (horasRestantes < 24 && horasRestantes > 0)
            {
                TempData["Error"] = $"No se puede cancelar con menos de 24 horas de anticipación. Faltan {horasRestantes:F1} horas.";
                return RedirectToAction(nameof(Index));
            }

            cita.Estado = "Cancelada";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cita cancelada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // GET: Citas/Reprogramar/5
        // MEJORADO: valida estado antes de permitir reprogramar
        // ============================================================
        public async Task<IActionResult> Reprogramar(int? id)
        {
            if (id == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (paciente == null) return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Medico).ThenInclude(m => m!.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id && c.PacienteId == paciente.Id);

            if (cita == null) return NotFound();

            // ⬇️ Solo se pueden reprogramar citas pendientes o confirmadas
            if (cita.Estado != "Pendiente" && cita.Estado != "Confirmada")
            {
                TempData["Error"] = $"No se puede reprogramar una cita con estado '{cita.Estado}'.";
                return RedirectToAction(nameof(Index));
            }

            var model = new CitaViewModel
            {
                Id = cita.Id,
                FechaHoraInicio = cita.FechaHoraInicio,
                MedicoId = cita.MedicoId,
                Tipo = cita.Tipo,
                Sala = cita.Sala,
                Motivo = cita.Motivo
            };

            model.MedicosDisponibles = await ObtenerMedicosDisponiblesAsync();
            return View(model);
        }

        // ============================================================
        // POST: Citas/Reprogramar
        // MEJORADO: mismas validaciones que Agendar
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reprogramar(CitaViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (paciente == null) return NotFound();

            if (ModelState.IsValid)
            {
                var cita = await _context.Citas
                    .FirstOrDefaultAsync(c => c.Id == model.Id && c.PacienteId == paciente.Id);

                if (cita == null) return NotFound();

                // ⬇️ Validar estado actual
                if (cita.Estado != "Pendiente" && cita.Estado != "Confirmada")
                {
                    ModelState.AddModelError("", $"No se puede reprogramar una cita con estado '{cita.Estado}'.");
                }
                // ⬇️ Validar fecha futura
                else if (model.FechaHoraInicio < DateTime.Now.AddMinutes(-5))
                {
                    ModelState.AddModelError("FechaHoraInicio", "No puede reprogramar para una fecha en el pasado.");
                }
                else
                {
                    // ⬇️ Validar conflicto de horarios
                    var inicioRango = model.FechaHoraInicio.AddMinutes(-30);
                    var finRango = model.FechaHoraInicio.AddMinutes(30);

                    var hayConflicto = await _context.Citas
                        .AnyAsync(c => c.MedicoId == model.MedicoId
                            && c.Id != model.Id  // ⬅️ Excluir la cita actual
                            && c.Estado != "Cancelada"
                            && c.FechaHoraInicio >= inicioRango
                            && c.FechaHoraInicio <= finRango);

                    if (hayConflicto)
                    {
                        ModelState.AddModelError("FechaHoraInicio",
                            "El médico ya tiene una cita programada en este horario.");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var cita = await _context.Citas
                    .FirstOrDefaultAsync(c => c.Id == model.Id && c.PacienteId == paciente.Id);

                if (cita == null) return NotFound();

                cita.FechaHoraInicio = model.FechaHoraInicio;
                cita.MedicoId = model.MedicoId!.Value;
                cita.Tipo = model.Tipo;
                cita.Sala = model.Tipo == "Virtual" ? "Consulta Virtual" : model.Sala;
                cita.Motivo = model.Motivo;
                cita.Estado = "Pendiente";

                await _context.SaveChangesAsync();

                TempData["Success"] = "Cita reprogramada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            model.MedicosDisponibles = await ObtenerMedicosDisponiblesAsync();
            return View(model);
        }

        // ============================================================
        // HELPER PRIVADO: Obtener médicos disponibles normalizados
        // SOLUCIONA LA DUPLICIDAD DE "Dr." DEFINITIVAMENTE
        // ============================================================
        private async Task<List<MedicoSelectItem>> ObtenerMedicosDisponiblesAsync()
        {
            var medicos = await _context.Medicos
                .Include(m => m.Usuario)
                .Include(m => m.Especialidad)
                .Where(m => m.EstaDisponible)
                .OrderBy(m => m.Especialidad!.Nombre)
                .ThenBy(m => m.Usuario!.NombreCompleto)
                .ToListAsync(); // Materializar para procesar en memoria

            return medicos.Select(m =>
            {
                // Normalizar el nombre para evitar duplicados "Dr. Dr. Juan"
                var nombreLimpio = (m.Usuario?.NombreCompleto ?? "")
                    .Replace("Dr. ", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("Dra. ", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("Dr ", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("Dra ", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();

                var tituloLimpio = (m.Titulo ?? "")
                    .Replace("Dr. Dr.", "Dr.", StringComparison.OrdinalIgnoreCase)
                    .Replace("Dra. Dra.", "Dra.", StringComparison.OrdinalIgnoreCase)
                    .Trim();

                var nombreFormateado = string.IsNullOrWhiteSpace(tituloLimpio)
                    ? nombreLimpio
                    : $"{tituloLimpio} {nombreLimpio}".Trim();

                return new MedicoSelectItem
                {
                    Id = m.Id,
                    NombreCompleto = nombreFormateado,
                    Titulo = tituloLimpio,
                    Especialidad = m.Especialidad?.Nombre ?? "General"
                };
            }).ToList();
        }
    }
}