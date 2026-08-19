using KLHealth.Web.Models.Entities;
using System.Collections.Generic;

namespace KLHealth.Web.Models.ViewModels
{
    public class ResultadosViewModel
    {
        public List<ResultadoMedico> TodosLosResultados { get; set; } = new List<ResultadoMedico>();
        public string FiltroActual { get; set; } = "Todos";
        public string TabActual { get; set; } = "Todos";

        // Contadores
        public int TotalResultados { get; set; }
        public int TotalDisponibles { get; set; }
        public int TotalPendientes { get; set; }
        public int TotalRecientes { get; set; }
        public int TotalMarcados { get; set; }

        // Datos para tarjetas inferiores
        public ResultadoMedico? UltimoResultado { get; set; }
        public int ResultadosEsteMes { get; set; }
    }
}