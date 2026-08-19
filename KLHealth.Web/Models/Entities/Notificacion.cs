using System.ComponentModel.DataAnnotations;

namespace KLHealth.Web.Models.Entities
{
    public class Notificacion
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        [Required, MaxLength(100)]
        public string Titulo { get; set; } = string.Empty;
        [Required, MaxLength(500)]
        public string Mensaje { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public bool Leida { get; set; } = false;
        [MaxLength(50)]
        public string Icono { get; set; } = "bi-bell"; // Clase de Bootstrap Icon
        [MaxLength(50)]
        public string Color { get; set; } = "text-primary"; // Clase de color de Bootstrap
    }
}