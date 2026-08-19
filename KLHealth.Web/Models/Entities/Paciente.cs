namespace KLHealth.Web.Models.Entities
{
    public class Paciente
    {
        public int Id { get; set; }
        public string? NumeroIdentificacion { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Pais { get; set; } = "Costa Rica";
        public string? NumeroPoliza { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public int? TipoSangreId { get; set; }
        public TipoSangre? TipoSangre { get; set; }
    }
}