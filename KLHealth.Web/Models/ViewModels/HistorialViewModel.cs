using KLHealth.Web.Models.Entities;
using System.Collections.Generic;

namespace KLHealth.Web.Models.ViewModels
{
    public class HistorialViewModel
    {
        public List<HistorialMedico> Registros { get; set; } = new List<HistorialMedico>();
        public string AnoSeleccionado { get; set; } = "Todos";
        public int TotalRegistros { get; set; }

        // Agrupación por mes/año
        public Dictionary<string, List<HistorialMedico>> RegistrosAgrupados { get; set; } = new Dictionary<string, List<HistorialMedico>>();

        // Años disponibles para filtros
        public List<int> AnosDisponibles { get; set; } = new List<int>();
    }
}