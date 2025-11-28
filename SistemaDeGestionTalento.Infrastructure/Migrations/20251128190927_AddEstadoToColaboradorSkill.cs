using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionTalento.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEstadoToColaboradorSkill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "colaboradores_skills",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "colaboradores_skills");
        }
    }
}
