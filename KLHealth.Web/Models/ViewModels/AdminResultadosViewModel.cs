using KLHealth.Web.Models.Entities;

namespace KLHealth.Web.Models.ViewModels
{
    public class AdminResultadosViewModel
    {
        public List<ResultadoMedico> Resultados { get; set; } = new List<ResultadoMedico>();
        public string TabActual { get; set; } = "Todos";
        public List<string> TabsDisponibles { get; set; } = new List<string>
        {
            "Todos", "Laboratorio", "Radiología", "Consultas"
        };
        public ResultadoMedico? ResultadoSeleccionado { get; set; }
        public int TotalResultados { get; set; }
        public int TotalPendientes { get; set; }
    }
}