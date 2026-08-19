using KLHealth.Web.Data;
using KLHealth.Web.Models.Entities;
using KLHealth.Web.Models.ViewModels;
using KLHealth.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KLHealth.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly KLHealthDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(KLHealthDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool Success, string Role, string Message)> LoginAsync(LoginViewModel model)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.EstaActivo);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Password, usuario.PasswordHash))
            {
                return (false, "", "Correo o contraseña incorrectos.");
            }

            // Crear Claims (Identidad del usuario)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await _httpContextAccessor.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Actualizar última conexión
            usuario.UltimaConexion = DateTime.Now;
            await _context.SaveChangesAsync();

            return (true, usuario.Rol.Nombre, "Inicio de sesión exitoso.");
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterViewModel model)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email))
            {
                return (false, "El correo electrónico ya está registrado.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Crear Usuario
                var nuevoUsuario = new Usuario
                {
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    NombreCompleto = $"{model.Nombre} {model.Apellido}",
                    Telefono = model.Telefono,
                    FechaNacimiento = model.FechaNacimiento,
                    RolId = 1, // 1 = Paciente
                    EstaActivo = true,
                    FechaRegistro = DateTime.Now
                };
                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                // 2. Crear registro de Paciente vinculado
                var nuevoPaciente = new Paciente
                {
                    UsuarioId = nuevoUsuario.Id,
                    NumeroIdentificacion = model.NumeroIdentificacion,
                    FechaNacimiento = model.FechaNacimiento,
                    Pais = "Costa Rica",
                    FechaRegistro = DateTime.Now
                };
                _context.Pacientes.Add(nuevoPaciente);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return (true, "Registro exitoso. Ahora puedes iniciar sesión.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return (false, "Ocurrió un error al registrar el usuario.");
            }
        }

        public async Task LogoutAsync()
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}