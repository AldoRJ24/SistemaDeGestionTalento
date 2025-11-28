using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace SistemaDeGestionTalento.Core.Entities
{
    [Table("colaboradores_skills")]
    public class ColaboradorSkill
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Usuario")]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;

        [ForeignKey("Skill")]
        [Column("skill_id")]
        public int SkillId { get; set; }
        public virtual Skill Skill { get; set; } = null!;

        [ForeignKey("NivelSkill")]
        [Column("nivel_id")]
        public int NivelId { get; set; }
        public virtual NivelSkill NivelSkill { get; set; } = null!;

        [Column("fecha_evaluacion")]
        public DateTime FechaEvaluacion { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Validado, Rechazado

        [ForeignKey("Evaluador")]
        [Column("evaluador_id")]
        public int? EvaluadorId { get; set; }
        public virtual Usuario? Evaluador { get; set; }
    }
}