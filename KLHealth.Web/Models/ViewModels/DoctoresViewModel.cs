using System.Collections.Generic;

namespace KLHealth.Web.Models.ViewModels
{
    public class DoctoresViewModel
    {
        public List<DoctorItem> Medicos { get; set; } = new List<DoctorItem>();
        public string Busqueda { get; set; } = string.Empty;
        public string EspecialidadFiltro { get; set; } = string.Empty;
        public List<string> EspecialidadesDisponibles { get; set; } = new List<string>();
        public int TotalResultados { get; set; }
    }

    public class DoctorItem
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public string FotoUrl { get; set; } = string.Empty;
        public string NumeroLicencia { get; set; } = string.Empty;
        public bool EstaDisponible { get; set; }
        public double Calificacion { get; set; }
        public int TotalResenas { get; set; }
        public string Experiencia { get; set; } = string.Empty;
    }
}