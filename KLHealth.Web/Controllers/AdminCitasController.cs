using KLHealth.Web.Data;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KLHealth.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminCitasController : Controller
    {
        private readonly KLHealthDbContext _context;

        public AdminCitasController(KLHealthDbContext context)
        {
            _context = context;
        }

        // =================================================================
        // 1. INDEX: Calendario Mensual y Cronograma
        // =================================================================
        public async Task<IActionResult> Index(int? mes, int? anio, int? medicoId)
        {
            ViewData["Title"] = "Agenda de Citas";

            // 1. Definir el mes y año a mostrar (por defecto, el actual)
            int mesActual = mes ?? DateTime.Now.Month;
            int anioActual = anio ?? DateTime.Now.Year;
            DateTime primerDiaMes = new DateTime(anioActual, mesActual, 1);
            DateTime primerDiaMesSiguiente = primerDiaMes.AddMonths(1);

            var model = new AdminCitasViewModel
            {
                MesActual = primerDiaMes,
                MedicoFiltroId = medicoId
            };

            // 2. Cargar lista de médicos para el filtro
            var medicosData = await _context.Medicos
                .Include(m => m.Usuario)
                .Where(m => m.EstaDisponible)
                .ToListAsync();

            // Filtrar y proyectar en memoria
            model.MedicosDisponibles = medicosData
                .Where(m => m.Usuario != null)
                .Select(m => new MedicoFiltroItem
                {
                    Id = m.Id,
                    NombreCompleto = $"{m.Titulo} {m.Usuario!.NombreCompleto}"
                })
                .OrderBy(m => m.NombreCompleto)
                .ToList();

            if (medicoId.HasValue)
            {
                var medicoSeleccionado = model.MedicosDisponibles.FirstOrDefault(m => m.Id == medicoId.Value);
                model.NombreMedicoFiltro = medicoSeleccionado?.NombreCompleto;
            }

            // 3. Consulta base de citas para el mes seleccionado
            var citasQuery = _context.Citas
                .Include(c => c.Paciente).ThenInclude(p => p!.Usuario)
                .Include(c => c.Medico).ThenInclude(m => m!.Usuario)
                .Where(c => c.FechaHoraInicio >= primerDiaMes && c.FechaHoraInicio < primerDiaMesSiguiente);

            // Aplicar filtro de médico si existe
            if (medicoId.HasValue)
            {
                citasQuery = citasQuery.Where(c => c.MedicoId == medicoId.Value);
            }

            var citasDelMes = await citasQuery.ToListAsync();

            // 4. Generar la cuadrícula del calendario (42 días: 6 semanas x 7 días)
            // Ajustar para que la semana empiece en Lunes (1) en lugar de Domingo (0)
            int diaSemanaPrimerDia = (int)primerDiaMes.DayOfWeek;
            if (diaSemanaPrimerDia == 0) diaSemanaPrimerDia = 7; // Convertir Domingo a 7

            DateTime diaInicioCalendario = primerDiaMes.AddDays(-(diaSemanaPrimerDia - 1));

            for (int i = 0; i < 42; i++)
            {
                DateTime diaActual = diaInicioCalendario.AddDays(i);

                var dayViewModel = new CalendarDayViewModel
                {
                    Fecha = diaActual,
                    EsHoy = diaActual.Date == DateTime.Today,
                    EsMesActual = diaActual.Month == mesActual
                };

                // Asignar citas a este día específico
                var citasDelDia = citasDelMes
                    .Where(c => c.FechaHoraInicio.Date == diaActual.Date)
                    .OrderBy(c => c.FechaHoraInicio)
                    .Select(c => MapearCitaResumen(c))
                    .ToList();

                dayViewModel.Citas = citasDelDia;
                model.DiasDelMes.Add(dayViewModel);
            }

            // 5. Datos para el Panel Lateral (Cronograma de HOY)
            DateTime hoy = DateTime.Today;
            DateTime manana = hoy.AddDays(1);

            var citasHoyQuery = _context.Citas
                .Include(c => c.Paciente).ThenInclude(p => p!.Usuario)
                .Include(c => c.Medico).ThenInclude(m => m!.Usuario)
                .Where(c => c.FechaHoraInicio >= hoy && c.FechaHoraInicio < manana);

            if (medicoId.HasValue)
            {
                citasHoyQuery = citasHoyQuery.Where(c => c.MedicoId == medicoId.Value);
            }

            var citasHoy = await citasHoyQuery.ToListAsync();

            model.TotalCitasHoy = citasHoy.Count;
            model.TotalConfirmadasHoy = citasHoy.Count(c => c.Estado == "Confirmada");

            model.CronogramaHoy = citasHoy
                .OrderBy(c => c.FechaHoraInicio)
                .Select(c => MapearCitaResumen(c))
                .ToList();

            return View(model);
        }

        // =================================================================
        // 2. DETALLES: Ver y cambiar estado de una cita
        // =================================================================
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Paciente).ThenInclude(p => p!.Usuario)
                .Include(c => c.Medico).ThenInclude(m => m!.Usuario)
                .Include(c => c.Medico).ThenInclude(m => m!.Especialidad)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cita == null) return NotFound();

            ViewData["Title"] = "Detalles de Cita";
            return View(cita);
        }

        // =================================================================
        // 3. CAMBIAR ESTADO: Actualizar estado de la cita (POST)
        // =================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null) return NotFound();

            // Validar estados permitidos
            var estadosValidos = new List<string> { "Pendiente", "Confirmada", "Completada", "Cancelada" };
            if (!estadosValidos.Contains(nuevoEstado))
            {
                TempData["Error"] = "Estado no válido.";
                return RedirectToAction(nameof(Detalles), new { id });
            }

            cita.Estado = nuevoEstado;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"El estado de la cita se actualizó a '{nuevoEstado}'.";
            return RedirectToAction(nameof(Detalles), new { id });
        }

        // =================================================================
        // MÉTODO AUXILIAR: Mapear Entidad a ViewModel
        // =================================================================
        private CitaResumenViewModel MapearCitaResumen(dynamic c)
        {
            // Determinar colores según el estado
            string bgClass = "bg-primary";
            string textClass = "text-white";

            switch (c.Estado)
            {
                case "Confirmada":
                    bgClass = "bg-success";
                    break;
                case "Pendiente":
                    bgClass = "bg-warning";
                    textClass = "text-dark";
                    break;
                case "Cancelada":
                    bgClass = "bg-danger";
                    break;
                case "Completada":
                    bgClass = "bg-secondary";
                    break;
            }

            return new CitaResumenViewModel
            {
                Id = c.Id,
                FechaHora = c.FechaHoraInicio,
                HoraFormateada = c.FechaHoraInicio.ToString("hh:mm tt"),
                PacienteNombre = c.Paciente?.Usuario?.NombreCompleto ?? "Paciente",
                Tipo = c.Tipo ?? "Presencial",
                Estado = c.Estado,
                Sala = c.Sala ?? "Consultorio",
                ColorBg = bgClass,
                ColorText = textClass
            };
        }
    }
}