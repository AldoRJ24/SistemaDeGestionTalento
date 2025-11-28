using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeGestionTalento.Core.Entities
{
    [Table("certificaciones")]
    public class Certificacion
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(150)]
        public string? Entidad { get; set; }

        [StringLength(500)]
        public string? Url { get; set; }

        public DateTime? FechaObtencion { get; set; }
    }
}