namespace KLHealth.Web.Models.ViewModels
{
    public class DashboardEstadisticas
    {
        public int TotalPacientes { get; set; }
        public int CitasHoy { get; set; }
        public int CitasPendientes { get; set; }
        public int CitasConfirmadas { get; set; }
        public int MedicosDisponibles { get; set; }
    }
}