namespace KLHealth.Web.Models.Entities
{
    public class Especialidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? IconoUrl { get; set; }
        public string? Color { get; set; }

        // Propiedades de navegación
        public ICollection<Medico> Medicos { get; set; } = new List<Medico>();
    }
}