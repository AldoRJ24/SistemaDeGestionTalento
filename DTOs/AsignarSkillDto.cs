using System.ComponentModel.DataAnnotations;

namespace SistemaDeGestionTalento.DTOs
{
    // Esta clase transporta los datos para asignar un skill
    public class AsignarSkillDto
    {
        [Required]
        public int SkillId { get; set; }

        [Required]
        public int NivelId { get; set; }

        // Opcional: El ID del líder que evalúa
        public int? EvaluadorId { get; set; }
    }
}