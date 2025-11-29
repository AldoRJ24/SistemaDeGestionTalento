namespace SistemaDeGestionTalento.Core.DTOs
{
    public class SkillMatchDetailDto
    {
        public string SkillNombre { get; set; } = string.Empty;
        public string NivelRequerido { get; set; } = string.Empty;
        public string NivelCandidato { get; set; } = "Ninguno";
        public double Porcentaje { get; set; } // 0 to 100
        public string Color { get; set; } = "red"; // red, orange, green
    }
}
