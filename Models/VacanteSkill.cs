using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <-- Asegúrate que este 'using' esté

namespace SistemaDeGestionTalento.Models
{
    [Table("vacante_skills")]
    public class VacanteSkill
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Vacante")]
        [Column("vacante_id")] // <-- MEJORA
        public int VacanteId { get; set; }
        public virtual Vacante Vacante { get; set; }

        [ForeignKey("Skill")]
        [Column("skill_id")] // <-- MEJORA
        public int SkillId { get; set; }
        public virtual Skill Skill { get; set; }

        [ForeignKey("NivelSkill")]
        [Column("nivel_id")] // <-- MEJORA
        public int NivelId { get; set; }
        public virtual NivelSkill NivelSkill { get; set; }
    }
}