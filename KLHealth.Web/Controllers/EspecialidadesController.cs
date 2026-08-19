using KLHealth.Web.Data;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KLHealth.Web.Controllers
{
    [Authorize(Roles = "Paciente")]
    public class EspecialidadesController : Controller
    {
        private readonly KLHealthDbContext _context;

        public EspecialidadesController(KLHealthDbContext context)
        {
            _context = context;
        }

        // GET: Especialidades
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Especialidades Médicas";

            // Consulta: Obtener todas las especialidades con el conteo de médicos asignados
            var especialidades = await _context.Especialidades
                .GroupJoin(
                    _context.Medicos.Where(m => m.EstaDisponible),
                    esp => esp.Id,
                    med => med.EspecialidadId,
                    (esp, medicos) => new EspecialidadListaViewModel
                    {
                        Id = esp.Id,
                        Nombre = esp.Nombre,
                        Descripcion = esp.Descripcion,
                        Icono = !string.IsNullOrEmpty(esp.IconoUrl) ? esp.IconoUrl : "bi-activity",
                        Color = !string.IsNullOrEmpty(esp.Color) ? esp.Color : "text-primary",
                        TotalDoctores = medicos.Count()
                    }
                )
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            return View(especialidades);
        }
    }
}