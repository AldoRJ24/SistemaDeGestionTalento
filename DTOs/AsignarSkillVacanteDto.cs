using System.ComponentModel.DataAnnotations;

namespace SistemaDeGestionTalento.DTOs
{
    // Esta clase transporta los datos para asignar un skill REQUERIDO a una vacante
    public class AsignarSkillVacanteDto
    {
        [Required]
        public int SkillId { get; set; }

        [Required]
        public int NivelId { get; set; }
    }
}