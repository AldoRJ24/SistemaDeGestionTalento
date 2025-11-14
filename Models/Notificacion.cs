using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeGestionTalento.Models
{
    [Table("notificaciones")]
    public class Notificacion
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("Vacante")]
        public int VacanteId { get; set; }
        public virtual Vacante Vacante { get; set; }

        [Required]
        public string Mensaje { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
        public bool Leido { get; set; } = false;
    }
}