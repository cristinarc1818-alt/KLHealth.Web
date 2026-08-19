using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KLHealth.Web.Models.ViewModels
{
    public class CitaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha y hora de la cita")]
        public DateTime FechaHoraInicio { get; set; }

        [Required(ErrorMessage = "El médico es obligatorio")]
        [Display(Name = "Médico")]
        public int? MedicoId { get; set; }

        [Required(ErrorMessage = "El tipo de cita es obligatorio")]
        [Display(Name = "Tipo de cita")]
        public string Tipo { get; set; } = "Presencial";

        [Display(Name = "Sala o ubicación")]
        public string? Sala { get; set; }

        [StringLength(500, ErrorMessage = "El motivo no puede exceder 500 caracteres")]
        [Display(Name = "Motivo de la consulta")]
        public string? Motivo { get; set; }

        public List<MedicoSelectItem> MedicosDisponibles { get; set; } = new List<MedicoSelectItem>();
    }

    public class MedicoSelectItem
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;   // ⬅️ AQUÍ (arregla CS1061)
        public string Especialidad { get; set; } = string.Empty;
    }
}