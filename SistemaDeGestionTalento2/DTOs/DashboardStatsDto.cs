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
        public List<TrendItemDto> SupplyDemandTrend { get; set; } = new List<TrendItemDto>();
    }

    public class StatItemDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TrendItemDto
    {
        public string Period { get; set; } = string.Empty; // e.g., "Jan 2023"
        public int Vacancies { get; set; }
        public int Collaborators { get; set; }
    }
}
