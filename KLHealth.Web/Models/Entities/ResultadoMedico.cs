using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KLHealth.Web.Models.Entities
{
    public class ResultadoMedico
    {
        public int Id { get; set; }

        // Clave foránea explícita para Paciente
        public int PacienteId { get; set; }

        [Required, MaxLength(100)]
        public string Tipo { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.Now;
        public bool Pendiente { get; set; } = true;
        public bool Marcado { get; set; } = false;

        public string? NombreExamen { get; set; }
        public string? ArchivoUrl { get; set; }

        // Clave foránea explícita para Médico
        public int? MedicoId { get; set; }

        // Propiedades de navegación CON atributos ForeignKey explícitos
        [ForeignKey("MedicoId")]
        public virtual Medico? Medico { get; set; }

        [ForeignKey("PacienteId")]
        public virtual Paciente? Paciente { get; set; }
    }
}