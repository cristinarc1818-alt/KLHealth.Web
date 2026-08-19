using System.ComponentModel.DataAnnotations;

namespace KLHealth.Web.Models.ViewModels
{
    public class EditarPerfilViewModel
    {
        public int UsuarioId { get; set; }
        public int PacienteId { get; set; }

        // Campos de solo lectura (se muestran pero no se editan)
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "ID del Paciente")]
        public string? NumeroIdentificacion { get; set; }

        // Campos editables con validaciones
        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        [Display(Name = "Número de Teléfono")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [StringLength(100, ErrorMessage = "La ciudad no puede exceder los 100 caracteres")]
        [Display(Name = "Ciudad")]
        public string? Ciudad { get; set; }

        [StringLength(10, ErrorMessage = "El código postal no puede exceder los 10 caracteres")]
        [Display(Name = "Código Postal")]
        public string? CodigoPostal { get; set; }

        [StringLength(100, ErrorMessage = "El país no puede exceder los 100 caracteres")]
        [Display(Name = "País")]
        public string? Pais { get; set; } = "Costa Rica";
    }
}