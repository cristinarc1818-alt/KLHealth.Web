using KLHealth.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KLHealth.Web.Data
{
    public static class DbInitializer
    {
        public static void Initialize(KLHealthDbContext context)
        {
            // Verificar si ya hay datos para no duplicarlos
            if (context.Roles.Any())
            {
                return;
            }

            // ==========================================
            // 1. SEMBRAR ROLES
            // ==========================================
            var rolPaciente = new Rol { Nombre = "Paciente", Descripcion = "Usuario paciente del sistema" };
            var rolMedico = new Rol { Nombre = "Medico", Descripcion = "Personal médico" };
            var rolAdmin = new Rol { Nombre = "Administrador", Descripcion = "Administrador del sistema" };

            context.Roles.AddRange(rolPaciente, rolMedico, rolAdmin);
            context.SaveChanges();

            // ==========================================
            // 2. SEMBRAR TIPOS DE SANGRE
            // ==========================================
            var sangreAPlus = new TipoSangre { Tipo = "A+" };
            var sangreAMinus = new TipoSangre { Tipo = "A-" };
            var sangreBPlus = new TipoSangre { Tipo = "B+" };
            var sangreBMinus = new TipoSangre { Tipo = "B-" };
            var sangreABPlus = new TipoSangre { Tipo = "AB+" };
            var sangreABMinus = new TipoSangre { Tipo = "AB-" };
            var sangreOPlus = new TipoSangre { Tipo = "O+" };
            var sangreOMinus = new TipoSangre { Tipo = "O-" };

            context.TiposSangre.AddRange(sangreAPlus, sangreAMinus, sangreBPlus, sangreBMinus,
                                         sangreABPlus, sangreABMinus, sangreOPlus, sangreOMinus);
            context.SaveChanges();

            // ==========================================
            // 3. SEMBRAR ESPECIALIDADES
            // ==========================================
            var espGeneral = new Especialidad { Nombre = "Medicina General", Descripcion = "Atención primaria y chequeos generales", Color = "#3b82f6" };
            var espCardio = new Especialidad { Nombre = "Cardiología", Descripcion = "Especialistas en el corazón y sistema cardiovascular", Color = "#ef4444" };
            var espPedia = new Especialidad { Nombre = "Pediatría", Descripcion = "Atención médica especializada para niños", Color = "#10b981" };
            var espDerma = new Especialidad { Nombre = "Dermatología", Descripcion = "Especialistas en piel, cabello y uñas", Color = "#f59e0b" };
            var espNeuro = new Especialidad { Nombre = "Neurología", Descripcion = "Atención especializada para el sistema nervioso", Color = "#8b5cf6" };
            var espGine = new Especialidad { Nombre = "Ginecología", Descripcion = "Salud femenina y sistema reproductiva", Color = "#ec4899" };
            var espOrto = new Especialidad { Nombre = "Ortopedia", Descripcion = "Tratamiento de lesiones y sistema musculoesquelético", Color = "#6366f1" };
            var espOdonto = new Especialidad { Nombre = "Odontología", Descripcion = "Cuidado dental completo y salud bucal", Color = "#14b8a6" };

            context.Especialidades.AddRange(espGeneral, espCardio, espPedia, espDerma,
                                            espNeuro, espGine, espOrto, espOdonto);
            context.SaveChanges();

            // ==========================================
            // 4. SEMBRAR USUARIOS
            // ==========================================
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");

            var usuarioAdmin = new Usuario
            {
                Email = "admin@klhealth.com",
                PasswordHash = hashedPassword,
                NombreCompleto = "Dr. Julian Vance (Admin)",
                Rol = rolAdmin,
                FechaRegistro = DateTime.Now
            };

            var usuarioMedico1 = new Usuario { Email = "medico@klhealth.com", PasswordHash = hashedPassword, NombreCompleto = "Dra. Sarah Miller", Rol = rolMedico, FechaRegistro = DateTime.Now };
            var usuarioMedico2 = new Usuario { Email = "medico2@klhealth.com", PasswordHash = hashedPassword, NombreCompleto = "Dr. Carlos Martínez", Rol = rolMedico, FechaRegistro = DateTime.Now };
            var usuarioMedico3 = new Usuario { Email = "medico3@klhealth.com", PasswordHash = hashedPassword, NombreCompleto = "Dra. Elena Ruiz", Rol = rolMedico, FechaRegistro = DateTime.Now };
            var usuarioMedico4 = new Usuario { Email = "medico4@klhealth.com", PasswordHash = hashedPassword, NombreCompleto = "Dr. Mario Gomez", Rol = rolMedico, FechaRegistro = DateTime.Now };
            var usuarioMedico5 = new Usuario { Email = "medico5@klhealth.com", PasswordHash = hashedPassword, NombreCompleto = "Dra. Lucía Méndez", Rol = rolMedico, FechaRegistro = DateTime.Now };
            var usuarioMedico6 = new Usuario { Email = "medico6@klhealth.com", PasswordHash = hashedPassword, NombreCompleto = "Dr. Roberto Sánchez", Rol = rolMedico, FechaRegistro = DateTime.Now };
            var usuarioMedico7 = new Usuario { Email = "medico7@klhealth.com", PasswordHash = hashedPassword, NombreCompleto = "Dra. Ana Torres", Rol = rolMedico, FechaRegistro = DateTime.Now };

            var usuarioPaciente1 = new Usuario { Email = "paciente@klhealth.com", PasswordHash = hashedPassword, NombreCompleto = "Alex Johnson", Rol = rolPaciente, FechaRegistro = DateTime.Now };
            var usuarioPaciente2 = new Usuario { Email = "paciente2@klhealth.com", PasswordHash = hashedPassword, NombreCompleto = "María García", Rol = rolPaciente, FechaRegistro = DateTime.Now };
            var usuarioPaciente3 = new Usuario { Email = "paciente3@klhealth.com", PasswordHash = hashedPassword, NombreCompleto = "Juan Pérez", Rol = rolPaciente, FechaRegistro = DateTime.Now };
            var usuarioPaciente4 = new Usuario { Email = "paciente4@klhealth.com", PasswordHash = hashedPassword, NombreCompleto = "Ana López", Rol = rolPaciente, FechaRegistro = DateTime.Now };
            var usuarioPaciente5 = new Usuario { Email = "paciente5@klhealth.com", PasswordHash = hashedPassword, NombreCompleto = "Carlos Rodríguez", Rol = rolPaciente, FechaRegistro = DateTime.Now };

            context.Usuarios.AddRange(usuarioAdmin, usuarioMedico1, usuarioMedico2, usuarioMedico3,
                                      usuarioMedico4, usuarioMedico5, usuarioMedico6, usuarioMedico7,
                                      usuarioPaciente1, usuarioPaciente2, usuarioPaciente3,
                                      usuarioPaciente4, usuarioPaciente5);
            context.SaveChanges();

            // ==========================================
            // 5. SEMBRAR MÉDICOS (con fotos de perfil)
            // ==========================================
            var medicos = new List<Medico>
            {
                new Medico { Usuario = usuarioMedico1, NumeroLicencia = "LIC-882910", Titulo = "Dra.", Especialidad = espCardio, EstaDeGuardia = true, EstaDisponible = true, AniosExperiencia = 12, CalificacionPromedio = 4.9m, FotoPerfilUrl = "https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=200&h=200&fit=crop&crop=face" },
                new Medico { Usuario = usuarioMedico2, NumeroLicencia = "LIC-772341", Titulo = "Dr.", Especialidad = espCardio, EstaDeGuardia = true, EstaDisponible = true, AniosExperiencia = 8, CalificacionPromedio = 4.8m, FotoPerfilUrl = "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?w=200&h=200&fit=crop&crop=face" },
                new Medico { Usuario = usuarioMedico3, NumeroLicencia = "LIC-990023", Titulo = "Dra.", Especialidad = espDerma, EstaDeGuardia = false, EstaDisponible = true, AniosExperiencia = 10, CalificacionPromedio = 4.7m, FotoPerfilUrl = "https://images.unsplash.com/photo-1594824476967-48c8b964273f?w=200&h=200&fit=crop&crop=face" },
                new Medico { Usuario = usuarioMedico4, NumeroLicencia = "LIC-112233", Titulo = "Dr.", Especialidad = espPedia, EstaDeGuardia = true, EstaDisponible = true, AniosExperiencia = 15, CalificacionPromedio = 5.0m, FotoPerfilUrl = "https://images.unsplash.com/photo-1622253692010-333f2da6031d?w=200&h=200&fit=crop&crop=face" },
                new Medico { Usuario = usuarioMedico5, NumeroLicencia = "LIC-445566", Titulo = "Dra.", Especialidad = espGeneral, EstaDeGuardia = true, EstaDisponible = true, AniosExperiencia = 6, CalificacionPromedio = 4.6m, FotoPerfilUrl = "https://images.unsplash.com/photo-1537368910025-700350fe46c7?w=200&h=200&fit=crop&crop=face" },
                new Medico { Usuario = usuarioMedico6, NumeroLicencia = "LIC-332211", Titulo = "Dr.", Especialidad = espNeuro, EstaDeGuardia = false, EstaDisponible = false, AniosExperiencia = 20, CalificacionPromedio = 4.9m, FotoPerfilUrl = "https://images.unsplash.com/photo-1582750433449-648ed127bb54?w=200&h=200&fit=crop&crop=face" },
                new Medico { Usuario = usuarioMedico7, NumeroLicencia = "LIC-667788", Titulo = "Dra.", Especialidad = espOrto, EstaDeGuardia = true, EstaDisponible = true, AniosExperiencia = 9, CalificacionPromedio = 4.8m, FotoPerfilUrl = "https://images.unsplash.com/photo-1651008376811-b90baee60c1f?w=200&h=200&fit=crop&crop=face" }
            };
            context.Medicos.AddRange(medicos);
            context.SaveChanges();

            // ==========================================
            // 6. SEMBRAR PACIENTES
            // ==========================================
            var pacientes = new List<Paciente>
            {
                new Paciente { Usuario = usuarioPaciente1, NumeroIdentificacion = "KL-8829", FechaNacimiento = new DateTime(1988, 5, 12), TipoSangre = sangreAPlus, Pais = "Costa Rica" },
                new Paciente { Usuario = usuarioPaciente2, NumeroIdentificacion = "KL-9021", FechaNacimiento = new DateTime(1992, 8, 24), TipoSangre = sangreOPlus, Pais = "Costa Rica" },
                new Paciente { Usuario = usuarioPaciente3, NumeroIdentificacion = "KL-1120", FechaNacimiento = new DateTime(1975, 3, 15), TipoSangre = sangreBPlus, Pais = "Costa Rica" },
                new Paciente { Usuario = usuarioPaciente4, NumeroIdentificacion = "KL-3345", FechaNacimiento = new DateTime(2001, 11, 5), TipoSangre = sangreABPlus, Pais = "Costa Rica" },
                new Paciente { Usuario = usuarioPaciente5, NumeroIdentificacion = "KL-5567", FechaNacimiento = new DateTime(1965, 1, 30), TipoSangre = sangreAMinus, Pais = "Costa Rica" }
            };
            context.Pacientes.AddRange(pacientes);
            context.SaveChanges();

            // ==========================================
            // 7. SEMBRAR CITAS DE PRUEBA
            // ==========================================
            var citas = new List<Cita>
            {
                new Cita
                {
                    Paciente = pacientes[0],
                    Medico = medicos[0],
                    FechaHoraInicio = DateTime.Now.AddDays(1).Date.AddHours(10),
                    Estado = "Confirmada",
                    Tipo = "Presencial",
                    Sala = "Consultorio 402",
                    Motivo = "Chequeo cardiovascular de rutina"
                },
                new Cita
                {
                    Paciente = pacientes[0],
                    Medico = medicos[2],
                    FechaHoraInicio = DateTime.Now.AddDays(7).Date.AddHours(16),
                    Estado = "Pendiente",
                    Tipo = "Virtual",
                    Sala = "Videollamada",
                    Motivo = "Revisión de lunares"
                },
                new Cita
                {
                    Paciente = pacientes[0],
                    Medico = medicos[4],
                    FechaHoraInicio = DateTime.Now.AddDays(-15).Date.AddHours(9),
                    Estado = "Completada",
                    Tipo = "Presencial",
                    Sala = "Consultorio 105",
                    Motivo = "Examen general anual"
                }
            };
            context.Citas.AddRange(citas);
            context.SaveChanges();

            // ==========================================
            // 8. SEMBRAR RESULTADOS MÉDICOS Y NOTIFICACIONES
            // ==========================================
            var resultados = new List<ResultadoMedico>
{
    new ResultadoMedico {
        PacienteId = 1,
        Tipo = "Laboratorio",
        NombreExamen = "Hemograma Completo (CBC)",
        Descripcion = "Análisis completo de células sanguíneas",
        Fecha = DateTime.Now.AddDays(-5),
        Pendiente = false,
        MedicoId = 1,
        ArchivoUrl = "/docs/hemograma.pdf"
    },
    new ResultadoMedico {
        PacienteId = 1,
        Tipo = "Laboratorio",
        NombreExamen = "Perfil Metabólico Integral",
        Descripcion = "Glucosa, electrolitos y función renal",
        Fecha = DateTime.Now.AddDays(-9),
        Pendiente = true,
        MedicoId = 2
    },
    new ResultadoMedico {
        PacienteId = 1,
        Tipo = "Radiología",
        NombreExamen = "Radiografía de Tórax (PA)",
        Descripcion = "Imagen de tórax vista posteroanterior",
        Fecha = DateTime.Now.AddDays(-11),
        Pendiente = false,
        MedicoId = 1,
        ArchivoUrl = "/docs/radiografia.pdf"
    },
    new ResultadoMedico {
        PacienteId = 1,
        Tipo = "Laboratorio",
        NombreExamen = "Perfil Lipídico",
        Descripcion = "Colesterol total, HDL, LDL y triglicéridos",
        Fecha = DateTime.Now.AddDays(-1),
        Pendiente = true,
        MedicoId = 2
    }
};
            context.ResultadosMedicos.AddRange(resultados);
            context.SaveChanges();

            var notificaciones = new List<Notificacion>
            {
                new Notificacion { PacienteId = 1, Titulo = "Resultado de laboratorio", Mensaje = "Tus resultados de sangre están listos para revisar.", Fecha = DateTime.Now.AddHours(-2), Leida = false, Icono = "bi-file-earmark-medical-fill", Color = "text-primary" },
                new Notificacion { PacienteId = 1, Titulo = "Cita confirmada", Mensaje = "Tu cita con Cardiología para mañana a las 10:00 AM fue confirmada.", Fecha = DateTime.Now.AddHours(-5), Leida = false, Icono = "bi-calendar-check-fill", Color = "text-success" },
                new Notificacion { PacienteId = 1, Titulo = "Receta renovada", Mensaje = "Tu receta de Lisinopril ha sido renovada y está disponible.", Fecha = DateTime.Now.AddDays(-1), Leida = true, Icono = "bi-capsule-fill", Color = "text-warning" }
            };
            context.Notificaciones.AddRange(notificaciones);
            context.SaveChanges();

            // ==========================================
            // 9. SEMBRAR HISTORIAL MÉDICO
            // ==========================================
            var historiales = new List<HistorialMedico>
            {
                new HistorialMedico {
                    PacienteId = 1,
                    MedicoId = 1,
                    FechaConsulta = DateTime.Now.AddMonths(-2),
                    TipoRegistro = "Tratamiento",
                    Titulo = "Sesión de Fisioterapia #4",
                    Descripcion = "Enfoque en la estabilización de la columna lumbar y fortalecimiento del núcleo. Aumento de resistencia en ejercicios de puente. El paciente reportó un 20% de mejora en la movilidad matutina.",
                    Ubicacion = "Centro de Rehabilitación Wellness"
                },
                new HistorialMedico {
                    PacienteId = 1,
                    MedicoId = 1,
                    FechaConsulta = DateTime.Now.AddMonths(-4),
                    TipoRegistro = "Receta",
                    Titulo = "Lisinopril 10mg",
                    Descripcion = "Medicamento de mantenimiento para la hipertensión. Tomar una tableta diaria por la mañana con agua.",
                    RecargasRestantes = 2
                },
                new HistorialMedico {
                    PacienteId = 1,
                    MedicoId = 1,
                    FechaConsulta = DateTime.Now.AddMonths(-4),
                    TipoRegistro = "Laboratorio",
                    Titulo = "Perfil Sanguíneo Anual",
                    Descripcion = "Análisis completo de sangre",
                    Valor1Nombre = "Colesterol",
                    Valor1Resultado = "190 mg/dL",
                    Valor2Nombre = "Azúcar en Sangre (Ayunas)",
                    Valor2Resultado = "95 mg/dL"
                },
                new HistorialMedico {
                    PacienteId = 1,
                    MedicoId = 5,
                    FechaConsulta = DateTime.Now.AddMonths(-5),
                    TipoRegistro = "Vacunacion",
                    Titulo = "Refuerzo de Tétanos (Tdap)",
                    Descripcion = "Siguiente refuerzo recomendado en 2034.",
                    ProximoRefuerzo = DateTime.Now.AddYears(10)
                },
                new HistorialMedico {
                    PacienteId = 1,
                    MedicoId = 5,
                    FechaConsulta = DateTime.Now.AddMonths(-6),
                    TipoRegistro = "Diagnostico",
                    Titulo = "Alergias Estacionales Leves",
                    Descripcion = "Diagnóstico de alergias estacionales",
                    Severidad = "Baja / Manejable",
                    EstadoRegistro = "Registro Activo",
                    NotasAdicionales = "MÉDICO DE CABECERA: Dr. Michael Chen"
                },
                new HistorialMedico {
                    PacienteId = 1,
                    MedicoId = 5,
                    FechaConsulta = DateTime.Now.AddMonths(-8),
                    TipoRegistro = "Consulta",
                    Titulo = "Consulta de Medicina General",
                    Descripcion = "Paciente acude por cuadro de 2 días de evolución con rinorrea y tos seca. Sin fiebre. Se indica tratamiento sintomático.",
                    Ubicacion = "Consultorio 105"
                }
            };
            
        }

    }
}