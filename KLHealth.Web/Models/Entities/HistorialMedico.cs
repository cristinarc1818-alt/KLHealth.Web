using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KLHealth.Web.Models.Entities
{
    public class HistorialMedico
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }

        [ForeignKey("PacienteId")]
        public Paciente? Paciente { get; set; }

        public int? MedicoId { get; set; }

        [ForeignKey("MedicoId")]
        public Medico? Medico { get; set; }

        public DateTime FechaConsulta { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string TipoRegistro { get; set; } = "Consulta"; // Tratamiento, Receta, Laboratorio, Vacunacion, Diagnostico

        [MaxLength(150)]
        public string Titulo { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Descripcion { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Ubicacion { get; set; }

        [MaxLength(500)]
        public string? NotasAdicionales { get; set; }

        // Datos específicos para Laboratorio
        public string? Valor1Nombre { get; set; }
        public string? Valor1Resultado { get; set; }
        public string? Valor2Nombre { get; set; }
        public string? Valor2Resultado { get; set; }

        // Datos específicos para Receta
        public int? RecargasRestantes { get; set; }

        // Datos específicos para Vacunación
        public DateTime? ProximoRefuerzo { get; set; }

        // Datos específicos para Diagnóstico
        public string? Severidad { get; set; }
        public string? EstadoRegistro { get; set; }
    }
}