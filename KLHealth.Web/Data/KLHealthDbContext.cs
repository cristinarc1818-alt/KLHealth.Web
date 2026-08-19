using KLHealth.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KLHealth.Web.Data
{
    public class KLHealthDbContext : DbContext
    {
        public KLHealthDbContext(DbContextOptions<KLHealthDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<TipoSangre> TiposSangre { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<ResultadoMedico> ResultadosMedicos { get; set; }
        public DbSet<HistorialMedico> HistorialesMedicos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasOne(e => e.Rol).WithMany(r => r.Usuarios).HasForeignKey(e => e.RolId);
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario).WithOne(u => u.Paciente).HasForeignKey<Paciente>(e => e.UsuarioId);
                entity.HasOne(e => e.TipoSangre).WithMany(t => t.Pacientes).HasForeignKey(e => e.TipoSangreId);
            });

            modelBuilder.Entity<Medico>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NumeroLicencia).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.NumeroLicencia).IsUnique();
                entity.HasOne(e => e.Usuario).WithOne(u => u.Medico).HasForeignKey<Medico>(e => e.UsuarioId);
                entity.HasOne(e => e.Especialidad).WithMany(m => m.Medicos).HasForeignKey(e => e.EspecialidadId);

                // Corrección para eliminar la advertencia de precisión decimal
                entity.Property(e => e.CalificacionPromedio).HasPrecision(18, 2);
                entity.Property(e => e.CostoConsulta).HasPrecision(18, 2);
            });

            modelBuilder.Entity<TipoSangre>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Tipo).IsRequired().HasMaxLength(10);
            });

            modelBuilder.Entity<Especialidad>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            // Configuración de HistorialMedico
            modelBuilder.Entity<HistorialMedico>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Paciente).WithMany().HasForeignKey(e => e.PacienteId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Medico).WithMany().HasForeignKey(e => e.MedicoId).OnDelete(DeleteBehavior.Restrict);
            });

            // CONFIGURACIÓN CORREGIDA DE CITA (Usa Restrict para evitar ciclos de cascada)
            modelBuilder.Entity<Cita>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Paciente).WithMany().HasForeignKey(e => e.PacienteId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Medico).WithMany().HasForeignKey(e => e.MedicoId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Notificacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne<Paciente>().WithMany().HasForeignKey(e => e.PacienteId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ResultadoMedico>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne<Paciente>().WithMany().HasForeignKey(e => e.PacienteId).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}