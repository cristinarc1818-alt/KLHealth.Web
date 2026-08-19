using KLHealth.Web.Models.ViewModels;

namespace KLHealth.Web.Models.ViewModels
{
    public class HospitalInfoViewModel
    {
        // Datos dinámicos desde la BD
        public List<EspecialidadListaViewModel> EspecialidadesDestacadas { get; set; } = new();
        public int TotalEspecialidades { get; set; }
        public int TotalDoctores { get; set; }

        // Datos estáticos (podrían venir de una tabla de configuración en el futuro)
        public string Direccion { get; set; } = "1221 Clinical Way, Medical District, San Francisco, CA 94103";
        public string TelefonoGeneral { get; set; } = "+1 (555) 123-4567";
        public string TelefonoEmergencias { get; set; } = "+1 (555) 999-0000";
    }
}