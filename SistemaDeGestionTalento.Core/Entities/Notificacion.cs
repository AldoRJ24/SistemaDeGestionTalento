using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeGestionTalento.Core.Entities
{
    [Table("notificaciones")]
    public class Notificacion
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;

        [ForeignKey("Vacante")]
        public int? VacanteId { get; set; }
        public virtual Vacante? Vacante { get; set; }

        [Required]
        public string Mensaje { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.Now;
        public bool Leido { get; set; } = false;

        public string Tipo { get; set; } = "General"; // General, Invitacion, Expulsion
        public string Estado { get; set; } = "Enviada"; // Enviada, Aceptada, Rechazada
    }
}