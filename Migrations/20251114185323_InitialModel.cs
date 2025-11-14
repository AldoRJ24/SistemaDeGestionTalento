using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionTalento.Migrations
{
    /// <inheritdoc />
    public partial class InitialModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           /* migrationBuilder.CreateTable(
                name: "kpi_logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreKpi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Valor = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kpi_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "niveles_skill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_niveles_skill", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "urgencias_vacante",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_urgencias_vacante", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContraseñaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    PuestoActual = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Disponibilidad = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usuarios_roles_RolId",
                        column: x => x.RolId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "certificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Entidad = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaObtencion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_certificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_certificaciones_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "colaboradores_skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    NivelId = table.Column<int>(type: "int", nullable: false),
                    FechaEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EvaluadorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_colaboradores_skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_colaboradores_skills_niveles_skill_NivelId",
                        column: x => x.NivelId,
                        principalTable: "niveles_skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_colaboradores_skills_skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_colaboradores_skills_usuarios_EvaluadorId",
                        column: x => x.EvaluadorId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_colaboradores_skills_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lider_colaborador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LiderId = table.Column<int>(type: "int", nullable: false),
                    ColaboradorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lider_colaborador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lider_colaborador_usuarios_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lider_colaborador_usuarios_LiderId",
                        column: x => x.LiderId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vacantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LiderId = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Proyecto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UrgenciaId = table.Column<int>(type: "int", nullable: true),
                    FechaInicioRequerida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vacantes_urgencias_vacante_UrgenciaId",
                        column: x => x.UrgenciaId,
                        principalTable: "urgencias_vacante",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_vacantes_usuarios_LiderId",
                        column: x => x.LiderId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "matching",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VacanteId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Porcentaje = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matching", x => x.Id);
                    table.ForeignKey(
                        name: "FK_matching_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_matching_vacantes_VacanteId",
                        column: x => x.VacanteId,
                        principalTable: "vacantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    VacanteId = table.Column<int>(type: "int", nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Leido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notificaciones_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notificaciones_vacantes_VacanteId",
                        column: x => x.VacanteId,
                        principalTable: "vacantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vacante_skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VacanteId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    NivelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacante_skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vacante_skills_niveles_skill_NivelId",
                        column: x => x.NivelId,
                        principalTable: "niveles_skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vacante_skills_skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vacante_skills_vacantes_VacanteId",
                        column: x => x.VacanteId,
                        principalTable: "vacantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_certificaciones_UsuarioId",
                table: "certificaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_colaboradores_skills_EvaluadorId",
                table: "colaboradores_skills",
                column: "EvaluadorId");

            migrationBuilder.CreateIndex(
                name: "IX_colaboradores_skills_NivelId",
                table: "colaboradores_skills",
                column: "NivelId");

            migrationBuilder.CreateIndex(
                name: "IX_colaboradores_skills_SkillId",
                table: "colaboradores_skills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_colaboradores_skills_UsuarioId_SkillId",
                table: "colaboradores_skills",
                columns: new[] { "UsuarioId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lider_colaborador_ColaboradorId",
                table: "lider_colaborador",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_lider_colaborador_LiderId_ColaboradorId",
                table: "lider_colaborador",
                columns: new[] { "LiderId", "ColaboradorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_matching_UsuarioId",
                table: "matching",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_matching_VacanteId",
                table: "matching",
                column: "VacanteId");

            migrationBuilder.CreateIndex(
                name: "IX_notificaciones_UsuarioId",
                table: "notificaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_notificaciones_VacanteId",
                table: "notificaciones",
                column: "VacanteId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_RolId",
                table: "usuarios",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_vacante_skills_NivelId",
                table: "vacante_skills",
                column: "NivelId");

            migrationBuilder.CreateIndex(
                name: "IX_vacante_skills_SkillId",
                table: "vacante_skills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_vacante_skills_VacanteId_SkillId",
                table: "vacante_skills",
                columns: new[] { "VacanteId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vacantes_LiderId",
                table: "vacantes",
                column: "LiderId");

            migrationBuilder.CreateIndex(
                name: "IX_vacantes_UrgenciaId",
                table: "vacantes",
                column: "UrgenciaId");
        */}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "certificaciones");

            migrationBuilder.DropTable(
                name: "colaboradores_skills");

            migrationBuilder.DropTable(
                name: "kpi_logs");

            migrationBuilder.DropTable(
                name: "lider_colaborador");

            migrationBuilder.DropTable(
                name: "matching");

            migrationBuilder.DropTable(
                name: "notificaciones");

            migrationBuilder.DropTable(
                name: "vacante_skills");

            migrationBuilder.DropTable(
                name: "niveles_skill");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropTable(
                name: "vacantes");

            migrationBuilder.DropTable(
                name: "urgencias_vacante");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
