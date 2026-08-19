using KLHealth.Web.Models.Entities;
using System.Collections.Generic;

namespace KLHealth.Web.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public string AdminName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;

        // KPIs
        public int TotalPacientes { get; set; }
        public int CitasHoy { get; set; }
        public int MedicosGuardia { get; set; }
        public decimal IngresosHoy { get; set; }

        // Tendencias
        public int PacientesNuevosMes { get; set; } = 12;
        public int MedicosCambio { get; set; } = -2;
        public decimal IngresosCambio { get; set; } = 8.4m;

        // Demografía
        public int TotalActivos { get; set; }
        public int Adultos { get; set; }
        public int Ninos { get; set; }
        public int Mayores { get; set; }

        // Listas
        public List<Cita> ProximasCitas { get; set; } = new List<Cita>();

        // Datos para gráfico de tendencias (últimos 12 meses)
        public List<int> DatosTendencias { get; set; } = new List<int>();
        public List<string> Meses { get; set; } = new List<string>();
    }
}