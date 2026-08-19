namespace KLHealth.Web.Models.ViewModels
{
    public class AdminCitasViewModel
    {
        // Navegación del calendario
        public DateTime MesActual { get; set; } = DateTime.Now;
        public List<CalendarDayViewModel> DiasDelMes { get; set; } = new List<CalendarDayViewModel>();

        // Filtros
        public int? MedicoFiltroId { get; set; }
        public string? NombreMedicoFiltro { get; set; }

        // Panel Lateral: Resumen
        public int TotalCitasHoy { get; set; }
        public int TotalConfirmadasHoy { get; set; }
        public List<CitaResumenViewModel> CronogramaHoy { get; set; } = new List<CitaResumenViewModel>();

        // Lista de médicos para el filtro
        public List<MedicoFiltroItem> MedicosDisponibles { get; set; } = new List<MedicoFiltroItem>();
    }

    public class CalendarDayViewModel
    {
        public DateTime Fecha { get; set; }
        public bool EsHoy { get; set; }
        public bool EsMesActual { get; set; }
        public List<CitaResumenViewModel> Citas { get; set; } = new List<CitaResumenViewModel>();
    }

    public class CitaResumenViewModel
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string HoraFormateada { get; set; } = string.Empty; // Ej: "09:30 AM"
        public string PacienteNombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty; // Presencial / Virtual
        public string Estado { get; set; } = string.Empty; // Pendiente, Confirmada, etc.
        public string Sala { get; set; } = string.Empty;

        // Clases CSS para colorear la cita en el calendario
        public string ColorBg { get; set; } = "bg-primary";
        public string ColorText { get; set; } = "text-white";
    }

    public class MedicoFiltroItem
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
    }
}