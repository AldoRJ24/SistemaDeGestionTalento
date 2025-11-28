using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionTalento.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescripcionPerfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "descripcion_perfil",
                table: "usuarios",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_by",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "descripcion_perfil",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "open_to_work",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "timestamp",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "validated_by",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "certificaciones");
        }
    }
}
