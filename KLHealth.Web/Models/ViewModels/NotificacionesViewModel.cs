namespace KLHealth.Web.Models.ViewModels
{
    public class NotificacionesViewModel
    {
        public List<NotificacionItem> Notificaciones { get; set; } = new List<NotificacionItem>();
        public int TotalSinLeer { get; set; }
        public int TotalCitasPendientes { get; set; }
        public int TotalResultadosNuevos { get; set; }
    }

    public class NotificacionItem
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty; // "Cita", "Resultado", "Paciente"
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public bool Leida { get; set; }
        public string? UrlAccion { get; set; }
        public string Icono { get; set; } = "bi-bell";
        public string ColorIcono { get; set; } = "#dbeafe";
        public string ColorTexto { get; set; } = "var(--blue-700)";
    }
}