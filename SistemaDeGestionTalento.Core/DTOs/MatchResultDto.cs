namespace SistemaDeGestionTalento.Core.DTOs
{
    public class MatchResultDto
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PuestoActual { get; set; } = string.Empty;
        public double PorcentajeCoincidencia { get; set; }
        
        // Breakdown
        public double ScoreTecnico { get; set; }
        public double ScoreHabilidadesBlandas { get; set; }
        public double ScoreSeniority { get; set; }

        public List<string> SkillsCoincidentes { get; set; } = new List<string>();
        public List<string> SkillsFaltantes { get; set; } = new List<string>();
    }
}
