using System.ComponentModel.DataAnnotations;

namespace KLHealth.Web.Models.ViewModels
{
    public class ConfiguracionViewModel
    {
        // Datos del Usuario (reales)
        public int UsuarioId { get; set; }

        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Dirección de Correo")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Display(Name = "ID del Paciente")]
        public string? NumeroIdentificacion { get; set; }

        // Preferencias de Notificaciones (UI only)
        [Display(Name = "Notificaciones por Correo")]
        public bool NotificacionesCorreo { get; set; } = true;

        [Display(Name = "Recordatorios por SMS")]
        public bool RecordatoriosSMS { get; set; } = true;

        // Seguridad (UI only)
        [Display(Name = "Autenticación de Dos Factores")]
        public bool AutenticacionDosFactores { get; set; } = true;

        [Display(Name = "Última Actualización de Contraseña")]
        public string UltimaActualizacionPassword { get; set; } = "Actualizada hace 3 meses";

        // Privacidad (UI only)
        [Display(Name = "Compartir Datos")]
        public bool CompartirDatos { get; set; } = false;

        // Apariencia (UI only)
        [Display(Name = "Idioma del Portal")]
        public string Idioma { get; set; } = "Español (Latinoamérica)";

        [Display(Name = "Tema")]
        public string Tema { get; set; } = "Claro";
    }
}