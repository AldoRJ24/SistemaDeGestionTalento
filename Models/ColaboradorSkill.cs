using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace SistemaDeGestionTalento.Models
{
    [Table("colaboradores_skills")]
    public class ColaboradorSkill
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Usuario")]
        [Column("usuario_id")] // <-- MEJORA
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("Skill")]
        [Column("skill_id")] // <-- MEJORA
        public int SkillId { get; set; }
        public virtual Skill Skill { get; set; }

        [ForeignKey("NivelSkill")]
        [Column("nivel_id")] // <-- MEJORA
        public int NivelId { get; set; }
        public virtual NivelSkill NivelSkill { get; set; }

        [Column("fecha_evaluacion")] // <-- MEJORA
        public DateTime FechaEvaluacion { get; set; } = DateTime.Now;

        [ForeignKey("Evaluador")]
        [Column("evaluador_id")] // <-- MEJORA
        public int? EvaluadorId { get; set; } // Nullable, por si es autoevaluado
        public virtual Usuario? Evaluador { get; set; }
    }
}