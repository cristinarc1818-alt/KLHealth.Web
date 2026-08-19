using KLHealth.Web.Models.Entities;
using System.Collections.Generic;

namespace KLHealth.Web.Models.ViewModels
{
    public class PacienteDashboardViewModel
    {
        public string NombrePaciente { get; set; } = string.Empty;
        public string IdPaciente { get; set; } = string.Empty;

        // Próxima cita
        public Cita? ProximaCita { get; set; }
        public string? MedicoNombre { get; set; }
        public string? EspecialidadNombre { get; set; }
        public string? FotoMedicoUrl { get; set; }
        public string? UbicacionCompleta { get; set; }
        public string? FechaHoraFormateada { get; set; }

        // KPIs
        public int TotalResultadosPendientes { get; set; }
        public int TotalRegistrosMedicos { get; set; }
        public int NotificacionesNuevas { get; set; }
        public string ResumenSalud { get; set; } = "Bueno";

        // Panel derecho
        public List<Notificacion> UltimasNotificaciones { get; set; } = new List<Notificacion>();
    }
}