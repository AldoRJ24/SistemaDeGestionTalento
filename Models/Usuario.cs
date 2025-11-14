using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <-- Asegúrate que este 'using' esté

namespace SistemaDeGestionTalento.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; }

        [Required]
        [StringLength(100)]
        public string Correo { get; set; }

        [Required]
        [Column("contraseña_hash")] // <-- MEJORA
        public string ContraseñaHash { get; set; }

        [ForeignKey("Rol")]
        [Column("rol_id")] // <-- MEJORA
        public int RolId { get; set; }
        public virtual Rol Rol { get; set; }

        [StringLength(100)]
        [Column("puesto_actual")] // <-- MEJORA
        public string? PuestoActual { get; set; }

        [StringLength(15)]
        public string Estado { get; set; } = "Activo";

        public bool Disponibilidad { get; set; } = true;

        [Column("fecha_creacion")] // <-- MEJORA
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

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