using KLHealth.Web.Data;
using KLHealth.Web.Models.Entities;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KLHealth.Web.Controllers
{
    [Authorize(Roles = "Paciente")]
    public class DoctoresController : Controller
    {
        private readonly KLHealthDbContext _context;

        public DoctoresController(KLHealthDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // GET: /Doctores
        // ============================================================
        public async Task<IActionResult> Index(string busqueda = "", string especialidad = "")
        {
            ViewData["Title"] = "Doctores Disponibles";

            var model = new DoctoresViewModel
            {
                Busqueda = busqueda,
                EspecialidadFiltro = especialidad
            };

            model.EspecialidadesDisponibles = await _context.Especialidades
                .Select(e => e.Nombre)
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync();

            var query = _context.Medicos
                .Include(m => m.Usuario)
                .Include(m => m.Especialidad)
                .Where(m => m.EstaDisponible);

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(m =>
                    m.Usuario.NombreCompleto.Contains(busqueda) ||
                    m.NumeroLicencia.Contains(busqueda) ||
                    m.Especialidad.Nombre.Contains(busqueda));
            }

            if (!string.IsNullOrWhiteSpace(especialidad))
            {
                query = query.Where(m => m.Especialidad.Nombre == especialidad);
            }

            var medicos = await query
                .OrderBy(m => m.Usuario.NombreCompleto)
                .Select(m => new DoctorItem
                {
                    Id = m.Id,
                    NombreCompleto = m.Usuario.NombreCompleto,
                    Titulo = m.Titulo,
                    Especialidad = m.Especialidad.Nombre,
                    FotoUrl = m.FotoPerfilUrl ?? "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?w=200&h=200&fit=crop&crop=face",
                    NumeroLicencia = m.NumeroLicencia,
                    EstaDisponible = m.EstaDisponible,
                    Calificacion = 4.8,
                    TotalResenas = 125,
                    Experiencia = "8 años"
                })
                .ToListAsync();

            model.Medicos = medicos;
            model.TotalResultados = medicos.Count;

            return View(model);
        }

        // ============================================================
        // GET: /Doctores/Detalles/5
        // ============================================================
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medico = await _context.Medicos
                .Include(m => m.Usuario)
                .Include(m => m.Especialidad)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medico == null)
            {
                return NotFound();
            }

            // Mapear al ViewModel de detalles
            var viewModel = new DoctorDetallesViewModel
            {
                Id = medico.Id,
                NombreCompleto = medico.Usuario.NombreCompleto,
                Titulo = medico.Titulo,
                Especialidad = medico.Especialidad.Nombre,
                Biografia = medico.Biografia,
                FotoPerfilUrl = medico.FotoPerfilUrl ?? "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?w=400&h=400&fit=crop&crop=face",
                NumeroLicencia = medico.NumeroLicencia,
                EstaDisponible = medico.EstaDisponible,
                Calificacion = 4.8,
                TotalResenas = 125,
                Experiencia = "8 años",
                Idiomas = new List<string> { "Español", "Inglés" },
                Horarios = new List<HorarioDto>
                {
                    new HorarioDto { Dia = "Lunes - Viernes", Hora = "08:00 - 17:00" },
                    new HorarioDto { Dia = "Sábados", Hora = "09:00 - 13:00" },
                    new HorarioDto { Dia = "Domingos", Hora = "No disponible" }
                },
                FormacionAcademica = new List<FormacionDto>
                {
                    new FormacionDto
                    {
                        Titulo = "Medicina General",
                        Institucion = "Universidad Nacional Autónoma",
                        Periodo = "2010 - 2016",
                        Icono = "bi-bank"
                    },
                    new FormacionDto
                    {
                        Titulo = $"Especialidad en {medico.Especialidad.Nombre}",
                        Institucion = "Hospital Universitario Central",
                        Periodo = "2017 - 2020",
                        Icono = "bi-book"
                    },
                    new FormacionDto
                    {
                        Titulo = "Certificación Internacional",
                        Institucion = "American Medical Association",
                        Periodo = "2021",
                        Icono = "bi-award"
                    }
                },
                Servicios = new List<string>
                {
                    "Consulta general",
                    "Diagnóstico especializado",
                    "Tratamiento personalizado",
                    "Seguimiento continuo",
                    "Segunda opinión médica",
                    "Teleconsulta"
                },
                Resenas = new List<ResenaDto>
                {
                    new ResenaDto
                    {
                        NombrePaciente = "María González",
                        Fecha = DateTime.Now.AddDays(-14),
                        Rating = 5,
                        Comentario = "Excelente profesional. Muy atento y explica todo con detalle. Totalmente recomendado."
                    },
                    new ResenaDto
                    {
                        NombrePaciente = "Carlos Ramírez",
                        Fecha = DateTime.Now.AddDays(-30),
                        Rating = 5,
                        Comentario = "El doctor me brindó un diagnóstico certero y un tratamiento muy efectivo. Muy agradecido."
                    },
                    new ResenaDto
                    {
                        NombrePaciente = "Ana Martínez",
                        Fecha = DateTime.Now.AddDays(-60),
                        Rating = 4,
                        Comentario = "Muy buen trato, aunque hubo un poco de espera. El doctor es muy profesional."
                    }
                }
            };

            ViewData["Title"] = $"{medico.Titulo} {medico.Usuario.NombreCompleto}";
            return View(viewModel);
        }
    }
}