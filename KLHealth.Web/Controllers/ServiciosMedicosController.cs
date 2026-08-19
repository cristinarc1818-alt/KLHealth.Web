using KLHealth.Web.Data;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KLHealth.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ServiciosMedicosController : Controller
    {
        private readonly KLHealthDbContext _context;

    public ServiciosMedicosController(KLHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string especialidad = "")
        {
            ViewData["Title"] = "Servicios Médicos";

            var query = _context.Medicos
                .Include(m => m.Especialidad)
                .Include(m => m.Usuario)
                .AsQueryable();

            // Filtro por especialidad
            if (!string.IsNullOrWhiteSpace(especialidad))
            {
                query = query.Where(m =>
                    m.Especialidad != null &&
                    m.Especialidad.Nombre == especialidad);
            }

            var medicos = await query.ToListAsync();

            var servicios = medicos.Select(m =>
            {
                // Obtener el nombre sin títulos para evitar:
                // "Dr. Dr. Juan Pérez"
                // "Dra. Dra. María López"
                var nombre = m.Usuario?.NombreCompleto?.Trim() ?? "";

                // Eliminar títulos que pudieran estar guardados
                // dentro de NombreCompleto.
                nombre = nombre
                    .Replace("Dr. ", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("Dra. ", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("Dr ", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("Dra ", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();

                // Obtener el título del médico.
                var titulo = m.Titulo?.Trim() ?? "";

                // Si el título no existe, no agregamos nada.
                string nombreMostrar;

                if (string.IsNullOrWhiteSpace(titulo))
                {
                    nombreMostrar = nombre;
                }
                else
                {
                    nombreMostrar = $"{titulo} {nombre}".Trim();
                }

                return new ServicioItem
                {
                    Id = m.Id,
                    Nombre = nombreMostrar,
                    Descripcion = m.Biografia,
                    Precio = m.CostoConsulta ?? 0,
                    Especialidad = m.Especialidad?.Nombre,
                    Disponible = m.EstaDisponible
                };
            }).ToList();

            var model = new ServiciosMedicosViewModel
            {
                Servicios = servicios,
                EspecialidadFiltro = especialidad
            };

            return View(model);
        }
    }

}
