using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeGestionTalento.Models
{
    [Table("niveles_skill")]
    public class NivelSkill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Nombre { get; set; }

        public int Orden { get; set; }
    }
}