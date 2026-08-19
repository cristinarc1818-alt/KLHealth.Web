using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KLHealth.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixResultadoMedicoRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "ResultadosMedicos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "PacienteId1",
                table: "ResultadosMedicos",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "Citas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosMedicos_PacienteId1",
                table: "ResultadosMedicos",
                column: "PacienteId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultadosMedicos_Pacientes_PacienteId1",
                table: "ResultadosMedicos",
                column: "PacienteId1",
                principalTable: "Pacientes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultadosMedicos_Pacientes_PacienteId1",
                table: "ResultadosMedicos");

            migrationBuilder.DropIndex(
                name: "IX_ResultadosMedicos_PacienteId1",
                table: "ResultadosMedicos");

            migrationBuilder.DropColumn(
                name: "PacienteId1",
                table: "ResultadosMedicos");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "ResultadosMedicos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "Citas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
