using System.ComponentModel.DataAnnotations;

namespace KLHealth.Web.Models.Entities
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        [StringLength(256, ErrorMessage = "El correo electrónico no puede exceder los 256 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(500, ErrorMessage = "La contraseña no puede exceder los 500 caracteres")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre completo no puede exceder los 100 caracteres")]
        public string NombreCompleto { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string? Telefono { get; set; }

        public DateTime? FechaNacimiento { get; set; }
        public int? GeneroId { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public DateTime? UltimaConexion { get; set; }
        public bool EstaActivo { get; set; } = true;
        public DateTime FechaUltimaModificacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public int RolId { get; set; }
        public Rol? Rol { get; set; }
        public Paciente? Paciente { get; set; }
        public Medico? Medico { get; set; }
    }
}