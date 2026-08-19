using KLHealth.Web.Data;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KLHealth.Web.Controllers
{
    [Authorize(Roles = "Paciente")]
    public class HospitalController : Controller
    {
        private readonly KLHealthDbContext _context;

        public HospitalController(KLHealthDbContext context)
        {
            _context = context;
        }

        // GET: Hospital
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Información del Hospital";

            // Consulta: Especialidades con conteo de médicos disponibles
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

            // Total de doctores disponibles en el hospital
            var totalDoctores = await _context.Medicos.CountAsync(m => m.EstaDisponible);

            var model = new HospitalInfoViewModel
            {
                EspecialidadesDestacadas = especialidades,
                TotalEspecialidades = especialidades.Count,
                TotalDoctores = totalDoctores
            };

            return View(model);
        }
    }
}