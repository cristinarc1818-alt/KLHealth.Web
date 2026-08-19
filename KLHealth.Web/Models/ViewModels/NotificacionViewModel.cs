using System.ComponentModel.DataAnnotations;

namespace KLHealth.Web.Models.ViewModels
{
    public class NotificacionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(100, ErrorMessage = "El título no puede exceder los 100 caracteres")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El mensaje es obligatorio")]
        [StringLength(500, ErrorMessage = "El mensaje no puede exceder los 500 caracteres")]
        [Display(Name = "Mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Icono")]
        public string Icono { get; set; } = "bi-bell";

        [Required]
        [Display(Name = "Tipo")]
        public string Color { get; set; } = "text-primary";
    }
}