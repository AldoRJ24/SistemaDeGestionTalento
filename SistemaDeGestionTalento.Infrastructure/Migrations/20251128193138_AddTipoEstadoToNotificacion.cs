using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionTalento.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoEstadoToNotificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "notificaciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "notificaciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "notificaciones");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "notificaciones");
        }
    }
}
