using System.ComponentModel.DataAnnotations;

namespace SistemaDeGestionTalento.DTOs
{
    public class CrearVacanteDto
    {
        [Required]
        public int LiderId { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(150)]
        public string? Proyecto { get; set; }

        public int? UrgenciaId { get; set; }

        public DateTime? FechaInicioRequerida { get; set; }

        public List<VacanteSkillDto> Skills { get; set; } = new List<VacanteSkillDto>();
    }
}