using System.ComponentModel.DataAnnotations;

namespace SistemaDeGestionTalento.DTOs
{
    public class UpdateUserProfileDto
    {
        [StringLength(100)]
        public string? Nombre { get; set; }

        [StringLength(100)]
        public string? Apellido { get; set; }

        [StringLength(100)]
        public string? PuestoActual { get; set; }

        public string? DescripcionPerfil { get; set; }

        public string? FotoPerfil { get; set; }
    }
}
