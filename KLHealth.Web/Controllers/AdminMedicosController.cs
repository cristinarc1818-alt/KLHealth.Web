using KLHealth.Web.Data;
using KLHealth.Web.Models.Entities;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace KLHealth.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminMedicosController : Controller
    {
        private readonly KLHealthDbContext _context;

        public AdminMedicosController(KLHealthDbContext context)
        {
            _context = context;
        }

        // =================================================================
        // 1. INDEX: Listado con filtros y paginación
        // =================================================================
        public async Task<IActionResult> Index(string especialidad = "", string busqueda = "", int pagina = 1)
        {
            ViewData["Title"] = "Gestión de Médicos";

            // 1. Obtener todas las especialidades para los tabs de filtro
            var especialidades = await _context.Especialidades
                .Select(e => e.Nombre)
                .OrderBy(e => e)
                .ToListAsync();

            // Agregar "Todos" al inicio para el filtro general
            var especialidadesConTodos = new List<string> { "Todos" };
            especialidadesConTodos.AddRange(especialidades);

            // 2. Consulta base de médicos
            var query = _context.Medicos
                .Include(m => m.Usuario)
                .Include(m => m.Especialidad)
                .AsQueryable();

            // 3. Aplicar filtros
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(m =>
                    m.Usuario.NombreCompleto.Contains(busqueda) ||
                    m.NumeroLicencia.Contains(busqueda) ||
                    (m.Especialidad != null && m.Especialidad.Nombre.Contains(busqueda)));
            }

            if (!string.IsNullOrWhiteSpace(especialidad) && especialidad != "Todos")
            {
                query = query.Where(m => m.Especialidad != null && m.Especialidad.Nombre == especialidad);
            }

            // 4. Paginación
            var totalRegistros = await query.CountAsync();
            var registrosPorPagina = 6; // Mostramos 6 por página (3x2 en el mockup)
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            if (pagina < 1) pagina = 1;
            if (pagina > totalPaginas && totalPaginas > 0) pagina = totalPaginas;

            var medicosPagina = await query
                .OrderBy(m => m.Usuario.NombreCompleto)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            // 5. Mapear a ViewModel y contar pacientes de hoy
            var medicosItems = new List<MedicoItem>();
            var hoy = DateTime.Today;

            foreach (var m in medicosPagina)
            {
                // Contar citas de hoy para este médico
                var pacientesHoy = await _context.Citas
                    .CountAsync(c => c.MedicoId == m.Id && c.FechaHoraInicio.Date == hoy && c.Estado != "Cancelada");

                medicosItems.Add(new MedicoItem
                {
                    Id = m.Id,
                    NombreCompleto = m.Usuario.NombreCompleto,
                    Titulo = m.Titulo ?? "Dr.",
                    Especialidad = m.Especialidad?.Nombre ?? "General",
                    NumeroLicencia = m.NumeroLicencia,
                    FotoPerfilUrl = m.FotoPerfilUrl,
                    EstaDeGuardia = m.EstaDeGuardia,
                    EstaDisponible = m.EstaDisponible,
                    AniosExperiencia = m.AniosExperiencia ?? 0,
                    CalificacionPromedio = m.CalificacionPromedio ?? 0,
                    PacientesHoy = pacientesHoy,
                    Email = m.Usuario.Email,
                    Telefono = m.Usuario.Telefono
                });
            }

            var model = new AdminMedicosViewModel
            {
                Medicos = medicosItems,
                EspecialidadFiltro = especialidad,
                Busqueda = busqueda,
                EspecialidadesDisponibles = especialidadesConTodos,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas,
                RegistrosPorPagina = registrosPorPagina,
                TotalRegistros = totalRegistros
            };

            return View(model);
        }

        // =================================================================
        // 2. CREAR: Mostrar formulario
        // =================================================================
        public async Task<IActionResult> Crear()
        {
            ViewData["Title"] = "Nuevo Médico";
            var model = new MedicoFormViewModel();
            await CargarEspecialidades(model);
            return View(model);
        }

        // =================================================================
        // 3. CREAR: Guardar en BD
        // =================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(MedicoFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarEspecialidades(model);
                return View(model);
            }

            // Validaciones de duplicidad
            if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "El correo electrónico ya está registrado");
                await CargarEspecialidades(model);
                return View(model);
            }

            if (await _context.Medicos.AnyAsync(m => m.NumeroLicencia == model.NumeroLicencia))
            {
                ModelState.AddModelError("NumeroLicencia", "El número de licencia ya está registrado");
                await CargarEspecialidades(model);
                return View(model);
            }

            // Obtener el Rol de Médico (Asumimos que existe en la tabla Roles)
            var rolMedico = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Medico");
            if (rolMedico == null)
            {
                TempData["Error"] = "Error de configuración: No se encontró el rol 'Medico'.";
                return RedirectToAction(nameof(Index));
            }

            // 1. Crear Usuario
            var usuario = new Usuario
            {
                NombreCompleto = model.NombreCompleto,
                Email = model.Email,
                Telefono = model.Telefono,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password ?? "Temporal123!"),
                RolId = rolMedico.Id,
                FechaRegistro = DateTime.Now,
                EstaActivo = true,
                FechaUltimaModificacion = DateTime.Now
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(); // Guardar para obtener el ID del usuario

            // 2. Crear Médico vinculado al Usuario
            var medico = new Medico
            {
                UsuarioId = usuario.Id,
                NumeroLicencia = model.NumeroLicencia,
                Titulo = model.Titulo,
                AniosExperiencia = model.AniosExperiencia,
                CostoConsulta = model.CostoConsulta,
                EspecialidadId = model.EspecialidadId,
                EstaDeGuardia = model.EstaDeGuardia,
                EstaDisponible = model.EstaDisponible,
                Biografia = model.Biografia,
                FotoPerfilUrl = "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?w=200&h=200&fit=crop&crop=face" // Foto por defecto
            };

            _context.Medicos.Add(medico);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Médico {model.NombreCompleto} creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // =================================================================
        // 4. EDITAR: Mostrar formulario
        // =================================================================
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null) return NotFound();

            var medico = await _context.Medicos
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medico == null) return NotFound();

            var model = new MedicoFormViewModel
            {
                MedicoId = medico.Id,
                UsuarioId = medico.UsuarioId,
                NombreCompleto = medico.Usuario.NombreCompleto,
                Email = medico.Usuario.Email,
                Telefono = medico.Usuario.Telefono,
                NumeroLicencia = medico.NumeroLicencia,
                Titulo = medico.Titulo ?? "Dr.",
                AniosExperiencia = medico.AniosExperiencia,
                CostoConsulta = medico.CostoConsulta,
                EspecialidadId = medico.EspecialidadId,
                EstaDeGuardia = medico.EstaDeGuardia,
                EstaDisponible = medico.EstaDisponible,
                Biografia = medico.Biografia
            };

            ViewData["Title"] = "Editar Médico";
            await CargarEspecialidades(model);
            return View(model);
        }

        // =================================================================
        // 5. EDITAR: Actualizar en BD
        // =================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, MedicoFormViewModel model)
        {
            if (id != model.MedicoId) return NotFound();

            var medico = await _context.Medicos
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medico == null) return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarEspecialidades(model);
                return View(model);
            }

            // Validar email único (excluyendo al usuario actual)
            if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email && u.Id != medico.UsuarioId))
            {
                ModelState.AddModelError("Email", "El correo electrónico ya está registrado por otro usuario");
                await CargarEspecialidades(model);
                return View(model);
            }

            // Actualizar datos del Usuario
            medico.Usuario.NombreCompleto = model.NombreCompleto;
            medico.Usuario.Email = model.Email;
            medico.Usuario.Telefono = model.Telefono;
            medico.Usuario.FechaUltimaModificacion = DateTime.Now;

            // Si se proporcionó una nueva contraseña, actualizarla
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                medico.Usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }

            // Actualizar datos del Médico
            medico.NumeroLicencia = model.NumeroLicencia;
            medico.Titulo = model.Titulo;
            medico.AniosExperiencia = model.AniosExperiencia;
            medico.CostoConsulta = model.CostoConsulta;
            medico.EspecialidadId = model.EspecialidadId;
            medico.EstaDeGuardia = model.EstaDeGuardia;
            medico.EstaDisponible = model.EstaDisponible;
            medico.Biografia = model.Biografia;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Médico actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =================================================================
        // 6. DETALLES: Ver perfil completo
        // =================================================================
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null) return NotFound();

            var medico = await _context.Medicos
                .Include(m => m.Usuario)
                .Include(m => m.Especialidad)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medico == null) return NotFound();

            // Calcular estadísticas rápidas para la vista de detalles
            var totalCitas = await _context.Citas.CountAsync(c => c.MedicoId == id);
            var citasCompletadas = await _context.Citas.CountAsync(c => c.MedicoId == id && c.Estado == "Completada");

            ViewBag.TotalCitas = totalCitas;
            ViewBag.CitasCompletadas = citasCompletadas;
            ViewData["Title"] = $"Detalles: {medico.Usuario.NombreCompleto}";

            return View(medico);
        }

        // =================================================================
        // 7. ELIMINAR: Borrar médico (POST)
        // =================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var medico = await _context.Medicos
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medico == null) return NotFound();

            // Validación de seguridad: No eliminar si tiene citas asociadas
            var tieneCitas = await _context.Citas.AnyAsync(c => c.MedicoId == id);
            if (tieneCitas)
            {
                TempData["Error"] = "No se puede eliminar el médico porque tiene citas registradas en el sistema. Se recomienda cambiar su estado a 'No Disponible'.";
                return RedirectToAction(nameof(Index));
            }

            // Eliminar Médico y Usuario asociado
            _context.Medicos.Remove(medico);
            _context.Usuarios.Remove(medico.Usuario);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Médico eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // =================================================================
        // MÉTODO AUXILIAR: Cargar especialidades en el ViewModel
        // =================================================================
        private async Task CargarEspecialidades(MedicoFormViewModel model)
        {
            model.Especialidades = await _context.Especialidades
                .Select(e => new EspecialidadSelectItem { Id = e.Id, Nombre = e.Nombre })
                .OrderBy(e => e.Nombre)
                .ToListAsync();
        }
    }
}