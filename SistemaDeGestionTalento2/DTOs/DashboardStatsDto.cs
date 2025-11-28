using System.Collections.Generic;

namespace SistemaDeGestionTalento.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalVacantes { get; set; }
        public int TotalColaboradores { get; set; }
        public List<StatItemDto> TopRequestedSkills { get; set; } = new List<StatItemDto>();
        public List<StatItemDto> TopCollaboratorSkills { get; set; } = new List<StatItemDto>();
        public List<StatItemDto> VacanciesByStatus { get; set; } = new List<StatItemDto>();
    }

    public class StatItemDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
