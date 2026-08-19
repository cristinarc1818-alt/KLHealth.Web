using KLHealth.Web.Models.Entities;
using System.Collections.Generic;

namespace KLHealth.Web.Models.ViewModels
{
    public class CitasListViewModel
    {
        public List<Cita> TodasLasCitas { get; set; } = new List<Cita>();
        public string FiltroActual { get; set; } = "Todas";

        
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int RegistrosPorPagina { get; set; } = 5; 
        public int TotalRegistros { get; set; }

        
        public int TotalCitas { get; set; }
        public int TotalPendientes { get; set; }
        public int TotalConfirmadas { get; set; }
        public int TotalCompletadas { get; set; }
        public int TotalCanceladas { get; set; }
    }
}