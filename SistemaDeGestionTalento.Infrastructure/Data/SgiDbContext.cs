using Microsoft.EntityFrameworkCore;
using SistemaDeGestionTalento.Core.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SistemaDeGestionTalento.Infrastructure.Data
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

            // --- Configurar DeleteBehavior para evitar ciclos (Error 1785) ---
            // Estrategia: Restrict para todo lo que pueda causar conflictos

            // ColaboradorSkill
            modelBuilder.Entity<ColaboradorSkill>()
                .HasOne(cs => cs.Usuario)
                .WithMany(u => u.ColaboradorSkills)
                .HasForeignKey(cs => cs.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ColaboradorSkill>()
                .HasOne(cs => cs.Skill)
                .WithMany(s => s.ColaboradorSkills) // Corrected from Colaboradores
                .HasForeignKey(cs => cs.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ColaboradorSkill>()
                .HasOne(cs => cs.NivelSkill)
                .WithMany()
                .HasForeignKey(cs => cs.NivelId)
                .OnDelete(DeleteBehavior.Restrict);

            // VacanteSkill
            modelBuilder.Entity<VacanteSkill>()
                .HasOne(vs => vs.Vacante)
                .WithMany(v => v.VacanteSkills)
                .HasForeignKey(vs => vs.VacanteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VacanteSkill>()
                .HasOne(vs => vs.Skill)
                .WithMany(s => s.VacanteSkills) // Corrected from Vacantes
                .HasForeignKey(vs => vs.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VacanteSkill>()
                .HasOne(vs => vs.NivelSkill)
                .WithMany()
                .HasForeignKey(vs => vs.NivelId)
                .OnDelete(DeleteBehavior.Restrict);

            // Certificacion
            modelBuilder.Entity<Certificacion>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Certificaciones)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Matching
            modelBuilder.Entity<Matching>()
                .HasOne(m => m.Vacante)
                .WithMany(v => v.Matchings)
                .HasForeignKey(m => m.VacanteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Matching>()
                .HasOne(m => m.Usuario)
                .WithMany(u => u.Matchings)
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notificacion
            modelBuilder.Entity<Notificacion>()
                .HasOne(n => n.Vacante)
                .WithMany(v => v.Notificaciones)
                .HasForeignKey(n => n.VacanteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notificacion>()
                .HasOne(n => n.Usuario)
                .WithMany(u => u.Notificaciones)
                .HasForeignKey(n => n.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}