using System;

namespace SistemaDeGestionTalento.DTOs
{
    public class CertificacionDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Entidad { get; set; }
        public string? Url { get; set; }
        public DateTime? FechaObtencion { get; set; }
    }
}
