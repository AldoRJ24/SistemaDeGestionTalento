using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeGestionTalento.Core.Entities
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [Column("contraseña_hash")]
        public string ContraseñaHash { get; set; } = string.Empty;

        [ForeignKey("Rol")]
        [Column("rol_id")]
        public int RolId { get; set; }
        public virtual Rol Rol { get; set; } = null!;

        [StringLength(100)]
        [Column("puesto_actual")]
        public string? PuestoActual { get; set; }

        [StringLength(15)]
        public string Estado { get; set; } = "Activo";

        public bool Disponibilidad { get; set; } = true;

        [Column("open_to_work")]
        public bool OpenToWork { get; set; } = false;

        [Column("descripcion_perfil")]
        public string? DescripcionPerfil { get; set; }

        [Column("foto_perfil")]
        public string? FotoPerfil { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // --- Audit Fields ---
        [StringLength(100)]
        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        [Column("updated_by")]
        public string? UpdatedBy { get; set; }

        [StringLength(100)]
        [Column("validated_by")]
        public string? ValidatedBy { get; set; }

        [Column("timestamp")]
        public DateTime? Timestamp { get; set; }

        // --- Propiedades de Navegación ---

        public virtual ICollection<ColaboradorSkill> ColaboradorSkills { get; set; } = new List<ColaboradorSkill>();
        public virtual ICollection<Certificacion> Certificaciones { get; set; } = new List<Certificacion>();
        public virtual ICollection<Vacante> VacantesCreadas { get; set; } = new List<Vacante>();
        public virtual ICollection<Matching> Matchings { get; set; } = new List<Matching>();
        public virtual ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();

        [InverseProperty("Lider")]
        public virtual ICollection<LiderColaborador> ColaboradoresAsignados { get; set; } = new List<LiderColaborador>();

        [InverseProperty("Colaborador")]
        public virtual ICollection<LiderColaborador> LideresAsignados { get; set; } = new List<LiderColaborador>();

        [InverseProperty("Evaluador")]
        public virtual ICollection<ColaboradorSkill> SkillsEvaluados { get; set; } = new List<ColaboradorSkill>();
    }
}