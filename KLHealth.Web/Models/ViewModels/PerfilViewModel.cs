using System.ComponentModel.DataAnnotations;

namespace KLHealth.Web.Models.ViewModels
{
    public class PerfilViewModel
    {
        // Datos del Usuario
        public int UsuarioId { get; set; }

        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Número de Teléfono")]
        public string? Telefono { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        // Datos del Paciente
        public int PacienteId { get; set; }

        [Display(Name = "ID del Paciente")]
        public string? NumeroIdentificacion { get; set; }

        [Display(Name = "Tipo de Sangre")]
        public string? TipoSangre { get; set; }

        // Campos simulados (no existen en BD aún)
        [Display(Name = "Contacto de Emergencia")]
        public string? ContactoEmergencia { get; set; } = "No registrado";

        [Display(Name = "Proveedor de Seguro")]
        public string? ProveedorSeguro { get; set; }

        [Display(Name = "Médico de Cabecera")]
        public string? MedicoCabecera { get; set; } = "No asignado";

        // Preferencias (UI only, sin persistencia)
        public bool NotificacionesPush { get; set; } = true;
        public bool AlertasSMS { get; set; } = false;
        public bool AutenticacionDosFactores { get; set; } = true;

        // Datos calculados
        public int Edad { get; set; }
        public string? FotoPerfilUrl { get; set; }
        public DateTime? UltimaVerificacion { get; set; }
    }
}