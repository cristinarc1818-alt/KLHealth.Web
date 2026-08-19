using System.ComponentModel.DataAnnotations;

namespace KLHealth.Web.Models.ViewModels
{
    public class MedicoFormViewModel
    {
        // IDs para edición
        public int? MedicoId { get; set; }
        public int? UsuarioId { get; set; }

        // Datos del Usuario
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string? ConfirmPassword { get; set; }

        // Datos del Médico
        [Required(ErrorMessage = "El número de licencia es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Número de Licencia")]
        public string NumeroLicencia { get; set; } = string.Empty;

        [Display(Name = "Título")]
        public string Titulo { get; set; } = "Dr.";

        [Range(0, 50)]
        [Display(Name = "Años de Experiencia")]
        public int? AniosExperiencia { get; set; }

        [Range(0, 10000)]
        [Display(Name = "Costo de Consulta")]
        public decimal? CostoConsulta { get; set; }

        [Display(Name = "Especialidad")]
        public int? EspecialidadId { get; set; }

        [Display(Name = "Está de Guardia")]
        public bool EstaDeGuardia { get; set; } = true;

        [Display(Name = "Está Disponible")]
        public bool EstaDisponible { get; set; } = true;

        [Display(Name = "Biografía")]
        public string? Biografia { get; set; }

        // Lista de especialidades para el dropdown
        public List<EspecialidadSelectItem> Especialidades { get; set; } = new List<EspecialidadSelectItem>();
    }

    public class EspecialidadSelectItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}