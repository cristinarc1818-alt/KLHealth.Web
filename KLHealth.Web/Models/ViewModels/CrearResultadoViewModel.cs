using System.ComponentModel.DataAnnotations;

namespace KLHealth.Web.Models.ViewModels
{
    public class CrearResultadoViewModel
    {
        [Required(ErrorMessage = "El paciente es obligatorio")]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "El médico es obligatorio")]
        public int? MedicoId { get; set; }

        [Required(ErrorMessage = "El tipo de examen es obligatorio")]
        [Display(Name = "Tipo de Examen")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del examen es obligatorio")]
        [MaxLength(200)]
        [Display(Name = "Nombre del Examen")]
        public string NombreExamen { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Display(Name = "URL del Archivo")]
        public string? ArchivoUrl { get; set; }

        // Listas para los dropdowns
        public List<SelectItem> Pacientes { get; set; } = new List<SelectItem>();
        public List<SelectItem> Medicos { get; set; } = new List<SelectItem>();
        public List<SelectItem> TiposExamen { get; set; } = new List<SelectItem>();
    }

    public class SelectItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}