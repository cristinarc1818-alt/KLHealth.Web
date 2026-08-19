using KLHealth.Web.Models.Entities;

namespace KLHealth.Web.Models.ViewModels
{
    public class AdminMedicosViewModel
    {
        // Lista de médicos a mostrar
        public List<MedicoItem> Medicos { get; set; } = new List<MedicoItem>();

        // Filtros
        public string EspecialidadFiltro { get; set; } = string.Empty;
        public string Busqueda { get; set; } = string.Empty;

        // Especialidades disponibles para los tabs
        public List<string> EspecialidadesDisponibles { get; set; } = new List<string>();

        // Paginación
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int RegistrosPorPagina { get; set; } = 6;
        public int TotalRegistros { get; set; }
    }

    public class MedicoItem
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public string NumeroLicencia { get; set; } = string.Empty;
        public string? FotoPerfilUrl { get; set; }
        public bool EstaDeGuardia { get; set; }
        public bool EstaDisponible { get; set; }
        public int AniosExperiencia { get; set; }
        public decimal CalificacionPromedio { get; set; }
        public int PacientesHoy { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Telefono { get; set; }
    }
}