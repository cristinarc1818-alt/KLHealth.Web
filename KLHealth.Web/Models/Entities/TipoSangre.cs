namespace KLHealth.Web.Models.Entities
{
    public class TipoSangre
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty; // "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"

        // Propiedades de navegación
        public ICollection<Paciente> Pacientes { get; set; } = new List<Paciente>();
    }
}