namespace KLHealth.Web.Models.ViewModels
{
    public class EspecialidadListaViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        // Propiedades visuales (con fallback por defecto)
        public string Icono { get; set; } = "bi-activity";
        public string Color { get; set; } = "text-primary";

        // Dato calculado: cantidad de médicos en esta especialidad
        public int TotalDoctores { get; set; }
    }
}