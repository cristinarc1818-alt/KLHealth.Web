using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KLHealth.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHistorialMedicoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Diagnostico",
                table: "HistorialesMedicos");

            migrationBuilder.DropColumn(
                name: "MotivoConsulta",
                table: "HistorialesMedicos");

            migrationBuilder.DropColumn(
                name: "NotasMedico",
                table: "HistorialesMedicos");

            migrationBuilder.RenameColumn(
                name: "Tratamiento",
                table: "HistorialesMedicos",
                newName: "Ubicacion");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "HistorialesMedicos",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EstadoRegistro",
                table: "HistorialesMedicos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotasAdicionales",
                table: "HistorialesMedicos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProximoRefuerzo",
                table: "HistorialesMedicos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecargasRestantes",
                table: "HistorialesMedicos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Severidad",
                table: "HistorialesMedicos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoRegistro",
                table: "HistorialesMedicos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Titulo",
                table: "HistorialesMedicos",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Valor1Nombre",
                table: "HistorialesMedicos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Valor1Resultado",
                table: "HistorialesMedicos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Valor2Nombre",
                table: "HistorialesMedicos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Valor2Resultado",
                table: "HistorialesMedicos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "HistorialesMedicos");

            migrationBuilder.DropColumn(
                name: "EstadoRegistro",
                table: "HistorialesMedicos");

            migrationBuilder.DropColumn(
                name: "NotasAdicionales",
                table: "HistorialesMedicos");

            migrationBuilder.DropColumn(
                name: "ProximoRefuerzo",
                table: "HistorialesMedicos");

            migrationBuilder.DropColumn(
                name: "RecargasRestantes",
                table: "HistorialesMedicos");

            migrationBuilder.DropColumn(
                name: "Severidad",
                table: "HistorialesMedicos");

            migrationBuilder.DropColumn(
                name: "TipoRegistro",
                table: "HistorialesMedicos");

            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "HistorialesMedicos");

            migrationBuilder.DropColumn(
                name: "Valor1Nombre",
                table: "HistorialesMedicos");

            migrationBuilder.DropColumn(
                name: "Valor1Resultado",
                table: "HistorialesMedicos");

            migrationBuilder.DropColumn(
                name: "Valor2Nombre",
                table: "HistorialesMedicos");

            migrationBuilder.DropColumn(
                name: "Valor2Resultado",
                table: "HistorialesMedicos");

            migrationBuilder.RenameColumn(
                name: "Ubicacion",
                table: "HistorialesMedicos",
                newName: "Tratamiento");

            migrationBuilder.AddColumn<string>(
                name: "Diagnostico",
                table: "HistorialesMedicos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MotivoConsulta",
                table: "HistorialesMedicos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NotasMedico",
                table: "HistorialesMedicos",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
