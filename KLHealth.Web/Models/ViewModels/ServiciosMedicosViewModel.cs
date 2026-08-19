namespace KLHealth.Web.Models.ViewModels
{
    public class ServiciosMedicosViewModel
    {
        public List<ServicioItem> Servicios { get; set; } = new List<ServicioItem>();
        public string? EspecialidadFiltro { get; set; }
    }

    public class ServicioItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string? Especialidad { get; set; }
        public bool Disponible { get; set; }
    }
}