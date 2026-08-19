using System;
using System.Collections.Generic;

namespace KLHealth.Web.Models.ViewModels
{
    public class DoctorDetallesViewModel
    {
        // Información básica
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public string? Biografia { get; set; }
        public string FotoPerfilUrl { get; set; } = string.Empty;
        public string NumeroLicencia { get; set; } = string.Empty;
        public bool EstaDisponible { get; set; }

        // Estadísticas
        public double Calificacion { get; set; }
        public int TotalResenas { get; set; }
        public string Experiencia { get; set; } = string.Empty;

        // Información adicional
        public List<string> Idiomas { get; set; } = new List<string>();
        public List<HorarioDto> Horarios { get; set; } = new List<HorarioDto>();
        public List<ResenaDto> Resenas { get; set; } = new List<ResenaDto>();
        public List<FormacionDto> FormacionAcademica { get; set; } = new List<FormacionDto>();
        public List<string> Servicios { get; set; } = new List<string>();

        // Nombre formateado (evita duplicados como "Dr. Dr. Juan")
        public string NombreFormateado
        {
            get
            {
                var nombre = NombreCompleto.Trim()
                    .Replace("Dr. ", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("Dra. ", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("Dr ", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("Dra ", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();

                var titulo = Titulo.Trim()
                    .Replace("Dr. Dr.", "Dr.", StringComparison.OrdinalIgnoreCase)
                    .Replace("Dra. Dra.", "Dra.", StringComparison.OrdinalIgnoreCase)
                    .Trim();

                return string.IsNullOrWhiteSpace(titulo)
                    ? nombre
                    : $"{titulo} {nombre}".Trim();
            }
        }
    }

    public class HorarioDto
    {
        public string Dia { get; set; } = string.Empty;
        public string Hora { get; set; } = string.Empty;
    }

    public class ResenaDto
    {
        public string NombrePaciente { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public int Rating { get; set; }
        public string Comentario { get; set; } = string.Empty;

        // Texto relativo (ej: "Hace 2 semanas")
        public string FechaRelativa
        {
            get
            {
                var dias = (DateTime.Now - Fecha).Days;
                if (dias == 0) return "Hoy";
                if (dias == 1) return "Ayer";
                if (dias < 7) return $"Hace {dias} días";
                if (dias < 30) return $"Hace {dias / 7} semanas";
                if (dias < 365) return $"Hace {dias / 30} meses";
                return $"Hace {dias / 365} años";
            }
        }
    }

    public class FormacionDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Institucion { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
        public string Icono { get; set; } = "bi-bank"; // Bootstrap Icon
    }
}