using KLHealth.Web.Data;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KLHealth.Web.Controllers
{
    [Authorize(Roles = "Paciente")]
    public class PerfilController : Controller
    {
        private readonly KLHealthDbContext _context;

        public PerfilController(KLHealthDbContext context)
        {
            _context = context;
        }

        // =================================================================
        // 1. GET: Perfil/Index
        // =================================================================
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Mi Perfil";

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var usuario = await _context.Usuarios
                .Include(u => u.Paciente)
                    .ThenInclude(p => p!.TipoSangre)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario?.Paciente == null)
            {
                return NotFound();
            }

            int edad = 0;
            if (usuario.FechaNacimiento.HasValue)
            {
                edad = DateTime.Now.Year - usuario.FechaNacimiento.Value.Year;
                if (DateTime.Now.DayOfYear < usuario.FechaNacimiento.Value.DayOfYear)
                    edad--;
            }

            var ultimoMedico = await _context.Citas
                .Include(c => c.Medico).ThenInclude(m => m!.Usuario)
                .Include(c => c.Medico).ThenInclude(m => m!.Especialidad)
                .Where(c => c.PacienteId == usuario.Paciente.Id && c.Estado == "Completada")
                .OrderByDescending(c => c.FechaHoraInicio)
                .Select(c => new {
                    Nombre = c.Medico!.Usuario.NombreCompleto,
                    Especialidad = c.Medico.Especialidad != null ? c.Medico.Especialidad.Nombre : "General"
                })
                .FirstOrDefaultAsync();

            string medicoCabecera = "No asignado";
            if (ultimoMedico != null)
            {
                medicoCabecera = $"Dr. {ultimoMedico.Nombre}, Especialista en {ultimoMedico.Especialidad}";
            }

            string proveedorSeguro = "No registrado";
            if (!string.IsNullOrEmpty(usuario.Paciente.NumeroPoliza))
            {
                proveedorSeguro = $"BlueCross Global Premium #{usuario.Paciente.NumeroPoliza}";
            }

            var model = new PerfilViewModel
            {
                UsuarioId = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                FechaNacimiento = usuario.FechaNacimiento,
                PacienteId = usuario.Paciente.Id,
                NumeroIdentificacion = usuario.Paciente.NumeroIdentificacion,
                TipoSangre = usuario.Paciente.TipoSangre?.Tipo,
                ProveedorSeguro = proveedorSeguro,
                MedicoCabecera = medicoCabecera,
                Edad = edad,
                UltimaVerificacion = DateTime.Now.AddDays(-15)
            };

            return View(model);
        }

        // =================================================================
        // 2. GET: Perfil/Editar
        // =================================================================
        public async Task<IActionResult> Editar()
        {
            ViewData["Title"] = "Editar Perfil";

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var usuario = await _context.Usuarios
                .Include(u => u.Paciente)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario?.Paciente == null)
            {
                return NotFound();
            }

            var model = new EditarPerfilViewModel
            {
                UsuarioId = usuario.Id,
                PacienteId = usuario.Paciente.Id,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                NumeroIdentificacion = usuario.Paciente.NumeroIdentificacion,
                Telefono = usuario.Telefono,
                FechaNacimiento = usuario.FechaNacimiento,
                Direccion = usuario.Paciente.Direccion,
                Ciudad = usuario.Paciente.Ciudad,
                CodigoPostal = usuario.Paciente.CodigoPostal,
                Pais = usuario.Paciente.Pais
            };

            return View(model);
        }

        // =================================================================
        // 3. POST: Perfil/Editar
        // =================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(EditarPerfilViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Paciente)
                .FirstOrDefaultAsync(u => u.Id == model.UsuarioId);

            if (usuario?.Paciente == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (usuario.Id != userId)
            {
                return Forbid();
            }

            usuario.Telefono = model.Telefono;
            usuario.FechaNacimiento = model.FechaNacimiento;
            usuario.FechaUltimaModificacion = DateTime.Now;

            usuario.Paciente.Direccion = model.Direccion;
            usuario.Paciente.Ciudad = model.Ciudad;
            usuario.Paciente.CodigoPostal = model.CodigoPostal;
            usuario.Paciente.Pais = model.Pais;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Perfil actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =================================================================
        // 4. GET: Perfil/CambiarPassword
        // =================================================================
        public IActionResult CambiarPassword()
        {
            ViewData["Title"] = "Cambiar Contraseña";
            return View(new CambiarPasswordViewModel());
        }

        // =================================================================
        // 5. POST: Perfil/CambiarPassword
        // =================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(CambiarPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _context.Usuarios.FindAsync(model.UsuarioId);
            if (usuario == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (usuario.Id != userId)
            {
                return Forbid();
            }

            if (!BCrypt.Net.BCrypt.Verify(model.PasswordActual, usuario.PasswordHash))
            {
                ModelState.AddModelError("PasswordActual", "La contraseña actual es incorrecta");
                return View(model);
            }

            if (BCrypt.Net.BCrypt.Verify(model.NuevaPassword, usuario.PasswordHash))
            {
                ModelState.AddModelError("NuevaPassword", "La nueva contraseña debe ser diferente a la actual");
                return View(model);
            }

            if (!EsPasswordFuerte(model.NuevaPassword))
            {
                ModelState.AddModelError("NuevaPassword", "La contraseña debe tener al menos 8 caracteres, una mayúscula, un número y un carácter especial");
                return View(model);
            }

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NuevaPassword);
            usuario.FechaUltimaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Contraseña cambiada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =================================================================
        // 6. Método auxiliar
        // =================================================================
        private bool EsPasswordFuerte(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool tieneMayuscula = false;
            bool tieneMinuscula = false;
            bool tieneNumero = false;
            bool tieneEspecial = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) tieneMayuscula = true;
                else if (char.IsLower(c)) tieneMinuscula = true;
                else if (char.IsDigit(c)) tieneNumero = true;
                else tieneEspecial = true;
            }

            return tieneMayuscula && tieneMinuscula && tieneNumero && tieneEspecial;
        }
    }
}