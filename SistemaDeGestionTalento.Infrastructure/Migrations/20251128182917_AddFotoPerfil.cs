using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionTalento.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFotoPerfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "foto_perfil",
                table: "usuarios",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "foto_perfil",
                table: "usuarios");
        }
    }
}
