using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KLHealth.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddResultadoMedicoField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArchivoUrl",
                table: "ResultadosMedicos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Marcado",
                table: "ResultadosMedicos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MedicoId",
                table: "ResultadosMedicos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreExamen",
                table: "ResultadosMedicos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosMedicos_MedicoId",
                table: "ResultadosMedicos",
                column: "MedicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultadosMedicos_Medicos_MedicoId",
                table: "ResultadosMedicos",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultadosMedicos_Medicos_MedicoId",
                table: "ResultadosMedicos");

            migrationBuilder.DropIndex(
                name: "IX_ResultadosMedicos_MedicoId",
                table: "ResultadosMedicos");

            migrationBuilder.DropColumn(
                name: "ArchivoUrl",
                table: "ResultadosMedicos");

            migrationBuilder.DropColumn(
                name: "Marcado",
                table: "ResultadosMedicos");

            migrationBuilder.DropColumn(
                name: "MedicoId",
                table: "ResultadosMedicos");

            migrationBuilder.DropColumn(
                name: "NombreExamen",
                table: "ResultadosMedicos");
        }
    }
}
