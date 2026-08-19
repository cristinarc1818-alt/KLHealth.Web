using KLHealth.Web.Data;
using KLHealth.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KLHealth.Web.Controllers
{
    [Authorize(Roles = "Medico,Administrador")]
    public class AdminController : Controller
    {
        private readonly KLHealthDbContext _context;

        public AdminController(KLHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = "Panel de Control";

            var userName = User.Identity?.Name ?? "Administrador";
            var userRole = User.IsInRole("Administrador") ? "Administrador" : "Médico";

            var model = new AdminDashboardViewModel
            {
                AdminName = userName,
                UserRole = userRole,
                ProximasCitas = await _context.Citas
                    .Include(c => c.Paciente).ThenInclude(p => p!.Usuario)
                    .Include(c => c.Medico).ThenInclude(m => m!.Usuario)
                    .Include(c => c.Medico).ThenInclude(m => m!.Especialidad)
                    .Where(c => c.FechaHoraInicio >= DateTime.Now && c.Estado != "Cancelada")
                    .OrderBy(c => c.FechaHoraInicio)
                    .Take(5)
                    .ToListAsync()
            };

           
            var estadisticas = new DashboardEstadisticas();

            
            using (var connection = _context.Database.GetDbConnection())
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_EstadisticasDashboard";
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            estadisticas.TotalPacientes = reader["TotalPacientes"] != DBNull.Value ? Convert.ToInt32(reader["TotalPacientes"]) : 0;
                            estadisticas.CitasHoy = reader["CitasHoy"] != DBNull.Value ? Convert.ToInt32(reader["CitasHoy"]) : 0;
                            estadisticas.CitasPendientes = reader["CitasPendientes"] != DBNull.Value ? Convert.ToInt32(reader["CitasPendientes"]) : 0;
                            estadisticas.CitasConfirmadas = reader["CitasConfirmadas"] != DBNull.Value ? Convert.ToInt32(reader["CitasConfirmadas"]) : 0;
                            estadisticas.MedicosDisponibles = reader["MedicosDisponibles"] != DBNull.Value ? Convert.ToInt32(reader["MedicosDisponibles"]) : 0;
                        }
                    }
                }
            }

            // Asignamos los datos del SP al modelo
            model.TotalPacientes = estadisticas.TotalPacientes;
            model.CitasHoy = estadisticas.CitasHoy;
            model.MedicosGuardia = estadisticas.MedicosDisponibles;

            // Datos para gráfico de tendencias (simulados para los últimos 12 meses)
            model.Meses = new List<string> { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
            model.DatosTendencias = new List<int> { 1800, 2100, 1950, 2300, 2400, 2482, 2200, 2050, 1900, 2150, 2350, 2100 };

            return View(model);
        }
    }
}