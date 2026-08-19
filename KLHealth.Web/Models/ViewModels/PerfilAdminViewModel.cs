using System.ComponentModel.DataAnnotations;

namespace KLHealth.Web.Models.ViewModels
{
    public class PerfilAdminViewModel
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string Rol { get; set; } = string.Empty;
        public string? Especialidad { get; set; }
        public bool EstaActivo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? FotoPerfilUrl { get; set; }
        public string? TelefonoEdit { get; set; }
        public string? NuevaContrasena { get; set; }
        public string? ConfirmarContrasena { get; set; }
    }
}