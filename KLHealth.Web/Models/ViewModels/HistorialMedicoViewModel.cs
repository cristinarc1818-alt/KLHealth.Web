using KLHealth.Web.Models.Entities;
using System.Collections.Generic;

namespace KLHealth.Web.Models.ViewModels
{
    public class HistorialMedicoViewModel
    {
        public Paciente? Paciente { get; set; }
        public List<Cita> Citas { get; set; } = new List<Cita>();
        public List<ResultadoMedico> Resultados { get; set; } = new List<ResultadoMedico>();

        // Comentado por si la tabla Alergias no existe en tu DbContext
        // public List<Alergia> Alergias { get; set; } = new List<Alergia>();

        public int? PacienteSeleccionadoId { get; set; }

        // Renombrado para evitar conflictos de ambigüedad
        public List<PacienteSelectorItem> PacientesDisponibles { get; set; } = new List<PacienteSelectorItem>();
    }

    public class PacienteSelectorItem
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string NumeroIdentificacion { get; set; } = string.Empty;
    }
}