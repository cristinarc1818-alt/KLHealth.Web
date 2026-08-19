using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KLHealth.Web.Models.Entities
{
    public class Cita
    {
        public int Id { get; set; }

        [Required]
        public DateTime FechaHoraInicio { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmada, Completada, Cancelada

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; } = "Presencial"; // Presencial, Virtual

        [StringLength(20)]
        public string? Sala { get; set; }

        public string? Motivo { get; set; }

        // Relaciones
        public int PacienteId { get; set; }
        public Paciente? Paciente { get; set; }

        public int MedicoId { get; set; }
        public Medico? Medico { get; set; }
    }
}