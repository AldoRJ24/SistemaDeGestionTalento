// --- USINGS REQUERIDOS ---
// Asegúrate de tener los paquetes NuGet instalados para que esto funcione
using Microsoft.EntityFrameworkCore;
// Importa la carpeta Models que creaste
using SistemaDeGestionTalento.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

// --- NAMESPACE DE TU CARPETA "Data" ---
namespace SistemaDeGestionTalento.Data
{
    public class SgiDbContext : DbContext
    {
        public SgiDbContext(DbContextOptions<SgiDbContext> options) : base(options)
        {
        }

        // Mapeo de Clases a DbSets (Tablas)
        public DbSet<Rol> Roles { get; set; }
        public DbSet<NivelSkill> NivelesSkill { get; set; }
        public DbSet<UrgenciaVacante> UrgenciasVacante { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Skill> Skills { get; set; }    
        public DbSet<ColaboradorSkill> ColaboradoresSkills { get; set; }
        public DbSet<LiderColaborador> LiderColaborador { get; set; }
        public DbSet<Vacante> Vacantes { get; set; }
        public DbSet<VacanteSkill> VacanteSkills { get; set; }
        public DbSet<Matching> Matchings { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Certificacion> Certificaciones { get; set; }
        public DbSet<KpiLog> KpiLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configurar Relaciones Complejas ---

            // Relación Lider-Colaborador (ambas FK apuntan a Usuario)
            modelBuilder.Entity<LiderColaborador>()
                .HasOne(lc => lc.Lider)
                .WithMany(u => u.ColaboradoresAsignados)
                .HasForeignKey(lc => lc.LiderId)
                .OnDelete(DeleteBehavior.Restrict); // Evitar borrado en cascada

            modelBuilder.Entity<LiderColaborador>()
                .HasOne(lc => lc.Colaborador)
                .WithMany(u => u.LideresAsignados)
                .HasForeignKey(lc => lc.ColaboradorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación del Evaluador (ColaboradorSkill)
            modelBuilder.Entity<ColaboradorSkill>()
                .HasOne(cs => cs.Evaluador)
                .WithMany(u => u.SkillsEvaluados)
                .HasForeignKey(cs => cs.EvaluadorId)
                .OnDelete(DeleteBehavior.SetNull); // Si se borra el evaluador, el registro queda

            // Relación de Vacante (creada por un Líder)
            modelBuilder.Entity<Vacante>()
                .HasOne(v => v.Lider)
                .WithMany(u => u.VacantesCreadas)
                .HasForeignKey(v => v.LiderId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Configurar Constraints Únicos (de tu SQL) ---

            modelBuilder.Entity<ColaboradorSkill>()
                .HasIndex(cs => new { cs.UsuarioId, cs.SkillId })
                .IsUnique();

            modelBuilder.Entity<LiderColaborador>()
                .HasIndex(lc => new { lc.LiderId, lc.ColaboradorId })
                .IsUnique();

            modelBuilder.Entity<VacanteSkill>()
                .HasIndex(vs => new { vs.VacanteId, vs.SkillId })
                .IsUnique();
        }
    }
}