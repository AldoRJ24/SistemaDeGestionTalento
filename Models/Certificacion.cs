using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeGestionTalento.Models
{
    [Table("certificaciones")]
    public class Certificacion
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; }

        [StringLength(150)]
        public string? Entidad { get; set; }
        public DateTime? FechaObtencion { get; set; }
    }
}