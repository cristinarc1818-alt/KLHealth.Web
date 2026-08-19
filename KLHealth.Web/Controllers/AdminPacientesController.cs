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
    public class AdminPacientesController : Controller
    {
        private readonly KLHealthDbContext _context;

        public AdminPacientesController(KLHealthDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> Index(string busqueda = "", int pagina = 1)
        {
            ViewData["Title"] = "Gestión de Pacientes";

            var query = _context.Pacientes
                .Include(p => p.Usuario)
                .AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(p =>
                    p.Usuario.NombreCompleto.Contains(busqueda) ||
                    p.Usuario.Email.Contains(busqueda) ||
                    p.NumeroIdentificacion.Contains(busqueda));
            }

            var totalRegistros = await query.CountAsync();
            var registrosPorPagina = 10;
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            if (pagina < 1) pagina = 1;
            if (pagina > totalPaginas && totalPaginas > 0) pagina = totalPaginas;

            var pacientes = await query
                .OrderByDescending(p => p.Usuario.FechaRegistro)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(p => new PacienteItem
                {
                    Id = p.Id,
                    NombreCompleto = p.Usuario.NombreCompleto,
                    Email = p.Usuario.Email,
                    NumeroIdentificacion = p.NumeroIdentificacion,
                    Telefono = p.Usuario.Telefono ?? "",
                    FechaNacimiento = p.FechaNacimiento,
                    Genero = "No especificado", // Valor por defecto ya que no existe en BD
                    FechaRegistro = p.Usuario.FechaRegistro,
                    TotalCitas = _context.Citas.Count(c => c.PacienteId == p.Id)
                })
                .ToListAsync();

            var model = new AdminPacientesViewModel
            {
                Pacientes = pacientes,
                Busqueda = busqueda,
                TotalRegistros = totalRegistros,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas,
                RegistrosPorPagina = registrosPorPagina
            };

            return View(model);
        }

        // CREATE: Mostrar formulario
        public IActionResult Crear()
        {
            ViewData["Title"] = "Nuevo Paciente";
            return View(new PacienteFormViewModel());
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(PacienteFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "El correo electrónico ya está registrado");
                    return View(model);
                }

                if (await _context.Pacientes.AnyAsync(p => p.NumeroIdentificacion == model.NumeroIdentificacion))
                {
                    ModelState.AddModelError("NumeroIdentificacion", "El número de identificación ya está registrado");
                    return View(model);
                }


                var usuario = new Usuario
                {
                    NombreCompleto = model.NombreCompleto,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    RolId = 3, 
                    Telefono = model.Telefono,
                    FechaRegistro = DateTime.Now
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Crear paciente
                var paciente = new Paciente
                {
                    UsuarioId = usuario.Id,
                    NumeroIdentificacion = model.NumeroIdentificacion,
                    FechaNacimiento = model.FechaNacimiento ?? DateTime.Now
                };

                _context.Pacientes.Add(paciente);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Paciente creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // EDIT: Mostrar formulario
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null) return NotFound();

            var paciente = await _context.Pacientes
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paciente == null) return NotFound();

            var model = new PacienteFormViewModel
            {
                Id = paciente.Id,
                NombreCompleto = paciente.Usuario.NombreCompleto,
                Email = paciente.Usuario.Email,
                NumeroIdentificacion = paciente.NumeroIdentificacion,
                Telefono = paciente.Usuario.Telefono ?? "",
                FechaNacimiento = paciente.FechaNacimiento,
                Genero = "No especificado",
                Password = "",
                ConfirmPassword = ""
            };

            ViewData["Title"] = "Editar Paciente";
            return View(model);
        }

        // EDIT: Actualizar paciente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, PacienteFormViewModel model)
        {
            if (id != model.Id) return NotFound();

            var paciente = await _context.Pacientes
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paciente == null) return NotFound();

            if (ModelState.IsValid)
            {
               
                if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email && u.Id != paciente.UsuarioId))
                {
                    ModelState.AddModelError("Email", "El correo electrónico ya está registrado");
                    return View(model);
                }

                
                if (await _context.Pacientes.AnyAsync(p => p.NumeroIdentificacion == model.NumeroIdentificacion && p.Id != id))
                {
                    ModelState.AddModelError("NumeroIdentificacion", "El número de identificación ya está registrado");
                    return View(model);
                }

              
                paciente.Usuario.NombreCompleto = model.NombreCompleto;
                paciente.Usuario.Email = model.Email;
                paciente.Usuario.Telefono = model.Telefono;

                // Actualizar contraseña si se proporcionó una nueva
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    paciente.Usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                }

                // Actualizar paciente
                paciente.NumeroIdentificacion = model.NumeroIdentificacion;
                paciente.FechaNacimiento = model.FechaNacimiento ?? DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Paciente actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // DETAILS: Ver detalles
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null) return NotFound();

            var paciente = await _context.Pacientes
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paciente == null) return NotFound();

            // Cargar las citas por separado
            var citas = await _context.Citas
                .Include(c => c.Medico).ThenInclude(m => m.Usuario)
                .Include(c => c.Medico).ThenInclude(m => m.Especialidad)
                .Where(c => c.PacienteId == id)
                .OrderByDescending(c => c.FechaHoraInicio)
                .Take(10)
                .ToListAsync();

            ViewBag.Citas = citas;

            ViewData["Title"] = $"Detalles: {paciente.Usuario.NombreCompleto}";
            return View(paciente);
        }

        // DELETE: Eliminar paciente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var paciente = await _context.Pacientes
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paciente == null) return NotFound();

            
            var totalCitas = await _context.Citas.CountAsync(c => c.PacienteId == id);
            if (totalCitas > 0)
            {
                TempData["Error"] = $"No se puede eliminar: El paciente tiene {totalCitas} cita(s) registradas.";
                return RedirectToAction(nameof(Index));
            }

           
            _context.Pacientes.Remove(paciente);

            
            var usuario = await _context.Usuarios.FindAsync(paciente.UsuarioId);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Paciente eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}