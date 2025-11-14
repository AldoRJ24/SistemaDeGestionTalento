using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeGestionTalento.Models
{
    [Table("skills")]
    public class Skill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        public string Categoria { get; set; }

        [StringLength(10)]
        public string Estado { get; set; } = "Activo";

        // --- Propiedades de Navegación ---
        public virtual ICollection<ColaboradorSkill> ColaboradorSkills { get; set; } = new List<ColaboradorSkill>();
        public virtual ICollection<VacanteSkill> VacanteSkills { get; set; } = new List<VacanteSkill>();
    }
}