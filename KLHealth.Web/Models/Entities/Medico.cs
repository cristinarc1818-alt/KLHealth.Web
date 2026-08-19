namespace KLHealth.Web.Models.Entities
{
    public class Medico
    {
        public int Id { get; set; }
        public string NumeroLicencia { get; set; } = string.Empty;
        public string? Titulo { get; set; } = "Dr.";
        public int? AniosExperiencia { get; set; }
        public decimal? CalificacionPromedio { get; set; } = 0.00m;
        public decimal? CostoConsulta { get; set; }
        public bool EstaDeGuardia { get; set; } = true;
        public bool EstaDisponible { get; set; } = true;
        public string? FotoPerfilUrl { get; set; }
        public string? Biografia { get; set; }
        public string? Idiomas { get; set; }

        // Propiedades de navegación
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public int? EspecialidadId { get; set; }
        public Especialidad? Especialidad { get; set; }
    }
}