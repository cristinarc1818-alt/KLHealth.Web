namespace KLHealth.Web.Models.Entities
{
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // "Paciente", "Medico", "Administrador"
        public string? Descripcion { get; set; }

        // Propiedades de navegación
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
